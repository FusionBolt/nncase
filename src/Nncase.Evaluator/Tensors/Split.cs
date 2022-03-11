// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using NetFabric.Hyperlinq;
using Nncase.IR;
using Nncase.IR.Math;
using Nncase.IR.Tensors;
using OrtKISharp;

namespace Nncase.Evaluator.Tensors;

/// <summary>
/// Evaluator for <see cref="Split"/>.
/// </summary>
public class SplitEvaluator : IEvaluator<Split>, ITypeInferencer<Split>
{
    /// <inheritdoc/>
    public IValue Visit(IEvaluateContext context, Split target)
    {
        var input = context.GetOrtArgumentValue(target, Split.Input);
        var split = context.GetOrtArgumentValue(target, Split.Sections);
        var axis = context.GetArgumentValueAsScalar<long>(target, Split.Axis);
        return Value.FromTensors(OrtKI.Split(input, split, axis).Select(t => t.ToTensor()).ToArray());
    }

    /// <inheritdoc/>
    public IRType Visit(ITypeInferenceContext context, Split target)
    {
        var input = context.CheckArgumentType<TensorType>(target, Split.Input);
        return Visit(context, target, input);
    }

    private IRType Visit(ITypeInferenceContext context, Split target, TensorType input)
    {
        if (context.GetArgument(target, Split.Axis) is TensorConst axis_con &&
            context.GetArgument(target, Split.Sections) is TensorConst sections_con)
        {
            var axis_v = axis_con.Value.ToScalar<int>();
            var sections_v = sections_con.Value.Cast<int>();
            var inshape = input.Shape.ToArray();
            if (inshape[axis_v] == Dimension.Unknown)
            {
                return new InvalidType("The Input Shape Axis Can Not Be Unknown!");
            }

            // split
            if (sections_v.Length == 1)
            {
                if (inshape[axis_v].FixedValue % sections_v[0] != 0)
                {
                    return new InvalidType("The Section Value Not Match Shape[Axis]!");
                }

                var outshape = new Dimension[inshape.Length];
                Array.Copy(inshape, outshape, inshape.Length);
                outshape[axis_v] = new Dimension(inshape[axis_v].FixedValue / sections_v[0]);
                return new TupleType(Enumerable.Repeat((IRType)(input with { Shape = new Shape(outshape) }), sections_v[0]));
            }
            else
            {
                if (sections_v.Sum() != inshape[axis_v].FixedValue)
                {
                    return new InvalidType("The Sections Sum Must Equal To Shape[Axis]!");
                }

                var outshape = new Dimension[inshape.Length];
                Array.Copy(inshape, outshape, inshape.Length);
                return new TupleType(from section in sections_v
                                     let x = outshape[axis_v] = section
                                     select input with { Shape = new Shape(outshape) });
            }
        }

        return new InvalidType("The Sections And Axis Must Be Const!");
    }
}
