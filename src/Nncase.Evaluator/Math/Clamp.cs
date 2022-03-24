// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using NetFabric.Hyperlinq;
using Nncase.CostModel;
using Nncase.IR;
using Nncase.IR.Math;
using Nncase.IR.Tensors;
using OrtKISharp;

namespace Nncase.Evaluator.Math;

/// <summary>
/// Evaluator for <see cref="Clamp"/>.
/// </summary>
public class ClampEvaluator : IEvaluator<Clamp>, ITypeInferencer<Clamp>, ICostEvaluator<Clamp>
{
    /// <inheritdoc/>
    public IValue Visit(IEvaluateContext context, Clamp clamp)
    {
        var input = context.GetOrtArgumentValue(clamp, Clamp.Input);
        var min = context.GetOrtArgumentValue(clamp, Clamp.Min);
        var max = context.GetOrtArgumentValue(clamp, Clamp.Max);
        return OrtKI.Clip(input, min, max).ToValue();
    }

    /// <inheritdoc/>
    public IRType Visit(ITypeInferenceContext context, Clamp target)
    {
        var input = context.CheckArgumentType<TensorType>(target, Clamp.Input);
        var min = context.CheckArgumentType<TensorType>(target, Clamp.Min);
        var max = context.CheckArgumentType<TensorType>(target, Clamp.Max);
        if (input.DType != min.DType || input.DType != max.DType || min.DType != max.DType)
        {
            return new InvalidType(
                $"clamp type is not equal, input:{input.DType}, min:${input.DType}, max:${input.DType}");
        }
        return Visit(input, min, max);
    }

    /// <inheritdoc/>
    public Cost Visit(ICostEvaluateContext context, Clamp target)
    {
        var returnType = context.GetReturnType<TensorType>();
        var arithm = returnType.Shape.Prod().FixedValue;
        return new(arithm, arithm * returnType.DType.SizeInBytes);
    }

    private IRType Visit(TensorType input, TensorType min, TensorType max)
    {
        return input;
    }
}
