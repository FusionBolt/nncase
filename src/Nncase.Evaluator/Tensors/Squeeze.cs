// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Linq;
using Nncase.IR;
using Nncase.IR.Tensors;
using static Tensorflow.Binding;
using torchF = TorchSharp.torch.nn.functional;

namespace Nncase.Evaluator.Tensors;

/// <summary>
/// Evaluator for <see cref="Squeeze"/>.
/// </summary>
public class SqueezeEvaluator : IEvaluator<Squeeze>, ITypeInferencer<Squeeze>
{
    /// <inheritdoc/>
    public IValue Visit(IEvaluateContext context, Squeeze squeeze)
    {
        var input = context.GetTFArgumentValue(squeeze, Squeeze.Input);
        var dims = context.GetArgumentValueAsTensor<int>(squeeze, Squeeze.Dim).ToArray();
        return tf.squeeze(input, dims).ToValue();
    }

    /// <inheritdoc/>
    public IRType Visit(ITypeInferenceContext context, Squeeze target)
    {
        var input = context.CheckArgumentType<TensorType>(target, Squeeze.Input);
        return Visit(context, target, input);
    }

    private IRType Visit(ITypeInferenceContext context, Squeeze target, TensorType input)
    {
        if (context.GetArgument(target, Squeeze.Dim) is TensorConst dim_con)
        {
            var dims = dim_con.Value.Cast<int>();
            var outshape = input.Shape.ToList();
            foreach (var dimValue in dims)
            {
                if (outshape[dimValue].IsFixed && outshape[dimValue] == 1)
                {
                    outshape[dimValue] = int.MaxValue;
                }
                else
                {
                    return new InvalidType("The Shape[dim] is not 1!");
                }
            }

            return input with { Shape = new Shape(outshape.Where(x => x != int.MaxValue)) };
        }

        return input with { Shape = new Shape(Enumerable.Repeat(Dimension.Unknown, input.Shape.Count() - 1)) };
    }
}