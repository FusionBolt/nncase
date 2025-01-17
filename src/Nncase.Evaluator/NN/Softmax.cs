// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using Nncase.CostModel;
using Nncase.IR;
using Nncase.IR.NN;
using OrtKISharp;

namespace Nncase.Evaluator.NN;

/// <summary>
/// Evaluator for <see cref="LogSoftmax"/>.
/// </summary>
public class LogSoftmaxEvaluator : IEvaluator<LogSoftmax>, ITypeInferencer<LogSoftmax>, ICostEvaluator<Softmax>
{
    /// <inheritdoc/>
    public IValue Visit(IEvaluateContext context, LogSoftmax logSoftMax)
    {
        var input = context.GetOrtArgumentValue(logSoftMax, LogSoftmax.Input);
        var axis = context.GetArgumentValueAsScalar<long>(logSoftMax, LogSoftmax.Axis);
        return OrtKI.LogSoftmax(input, axis).ToValue();
    }

    /// <inheritdoc/>
    public IRType Visit(ITypeInferenceContext context, LogSoftmax target)
    {
        var input = context.CheckArgumentType<TensorType>(target, LogSoftmax.Input);
        return Visit(input);
    }

    /// <inheritdoc/>
    public Cost? Visit(ICostEvaluateContext context, Softmax target)
    {
        var ret = context.GetReturnType<TensorType>();
        var macPerElement = 4;
        return new()
        {
            [CostFactorNames.MemoryLoad] = CostUtility.GetMemoryAccess(ret),
            [CostFactorNames.MemoryStore] = CostUtility.GetMemoryAccess(ret),
            [CostFactorNames.CPUCycles] = CostUtility.GetCPUCycles(ret, macPerElement),
        };
    }

    private IRType Visit(TensorType input)
    {
        return input;
    }
}

/// <summary>
/// Evaluator for <see cref="Softmax"/>.
/// </summary>
public class SoftmaxEvaluator : IEvaluator<Softmax>, ITypeInferencer<Softmax>
{
    /// <inheritdoc/>
    public IValue Visit(IEvaluateContext context, Softmax softMax)
    {
        var input = context.GetOrtArgumentValue(softMax, Softmax.Input);
        var dim = context.GetArgumentValueAsScalar<int>(softMax, Softmax.Axis);
        return OrtKI.Softmax(input, dim).ToValue();
    }

    /// <inheritdoc/>
    public IRType Visit(ITypeInferenceContext context, Softmax target)
    {
        var input = context.CheckArgumentType<TensorType>(target, Softmax.Input);
        return Visit(input);
    }

    private IRType Visit(TensorType input)
    {
        return input;
    }
}

/// <summary>
/// Evaluator for <see cref="Softplus"/>.
/// </summary>
public class SoftplusEvaluator : IEvaluator<Softplus>, ITypeInferencer<Softplus>
{
    /// <inheritdoc/>
    public IValue Visit(IEvaluateContext context, Softplus softPlus)
    {
        var input = context.GetOrtArgumentValue(softPlus, Softplus.Input);
        return OrtKI.Softplus(input).ToValue();
    }

    /// <inheritdoc/>
    public IRType Visit(ITypeInferenceContext context, Softplus target)
    {
        var input = context.CheckArgumentType<TensorType>(target, Softplus.Input);
        return Visit(input);
    }

    private IRType Visit(TensorType input)
    {
        return input;
    }
}

/// <summary>
/// Evaluator for <see cref="Softsign"/>.
/// </summary>
public class SoftsignEvaluator : IEvaluator<Softsign>, ITypeInferencer<Softsign>
{
    /// <inheritdoc/>
    public IValue Visit(IEvaluateContext context, Softsign softSign)
    {
        var input = context.GetOrtArgumentValue(softSign, Softsign.Input);
        return OrtKI.Softsign(input).ToValue();
    }

    /// <inheritdoc/>
    public IRType Visit(ITypeInferenceContext context, Softsign target)
    {
        var input = context.CheckArgumentType<TensorType>(target, Softsign.Input);
        return Visit(input);
    }

    private IRType Visit(TensorType input)
    {
        return input;
    }
}
