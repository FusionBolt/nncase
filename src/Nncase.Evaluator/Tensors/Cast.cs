// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using Nncase.IR;
using Nncase.IR.Tensors;
using OrtKISharp;

namespace Nncase.Evaluator.Tensors;

/// <summary>
/// Evaluator for <see cref="Cast"/>.
/// </summary>
public class CastEvaluator : IEvaluator<Cast>, ITypeInferencer<Cast>
{
    /// <inheritdoc/>
    public IValue Visit(IEvaluateContext context, Cast cast)
    {
        var input = context.GetArgumentValue(cast, Cast.Input).AsTensor();
        return Value.FromTensor(input.CastTo(cast.NewType));
    }

    /// <inheritdoc/>
    public IRType Visit(ITypeInferenceContext context, Cast target)
    {
        var input = context.CheckArgumentType<TensorType>(target, Cast.Input);
        return Visit(target, input);
    }

    private IRType Visit(Cast target, TensorType input)
    {
        return new TensorType(target.NewType, input.Shape);
    }
}
