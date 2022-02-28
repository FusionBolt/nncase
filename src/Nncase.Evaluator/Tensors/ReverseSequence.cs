// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using Nncase.IR;
using Nncase.IR.Tensors;
using Tensorflow;
using static Tensorflow.Binding;

namespace Nncase.Evaluator.Tensors;

/// <summary>
/// Evaluator for <see cref="ReverseSequence"/>.
/// </summary>
public class ReverseSequenceEvaluator : IEvaluator<ReverseSequence>, ITypeInferencer<ReverseSequence>
{
    /// <inheritdoc/>
    public IValue Visit(IEvaluateContext context, ReverseSequence random)
    {
        var input = context.GetTFArgumentValue(random, ReverseSequence.Input);
        var seqLens = context.GetTFArgumentValue(random, ReverseSequence.SeqLens);
        var batchAxis = context.GetArgumentValueAsScalar<int>(random, ReverseSequence.BatchAxis);
        var timeAxis = context.GetArgumentValueAsScalar<int>(random, ReverseSequence.TimeAxis);
        return tf.Context.ExecuteOp(
            "ReverseSequence",
            null!,
            new ExecuteOpArgs(input, seqLens)
                .SetAttributes(new
                {
                    seq_dim = timeAxis,
                    batch_dim = batchAxis,
                }))[0].ToValue();
    }

    /// <inheritdoc/>
    public IRType Visit(ITypeInferenceContext context, ReverseSequence target)
    {
        var input = context.CheckArgumentType<TensorType>(target, Reshape.Input);
        return Visit(context, target, input);
    }

    private IRType Visit(ITypeInferenceContext context, ReverseSequence target, TensorType input)
    {
        return input;
    }
}