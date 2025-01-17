// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using Nncase.CostModel;
using Nncase.IR;
using Nncase.IR.Math;
using OrtKISharp;

namespace Nncase.Evaluator.Math;

/// <summary>
/// Evaluator for <see cref="Require"/>.
/// </summary>

[PatternMatch.PatternFunctionalGenerator, TypeInferGenerator, EvaluatorGenerator]
public partial class RequireEvaluator : IEvaluator<Require>, ITypeInferencer<Require>, IOpPrinter<Require>
{
    /// <inheritdoc />
    IValue Visit(bool Predicate, IValue Value, Require target)
    {
        if (!Predicate)
            throw new InvalidOperationException($"The Require {target.Message} Is False!");
        return Value;
    }

    /// <inheritdoc/>
    IRType Visit(TensorType Predicate, IRType Value)
    {
        return Value;
    }

    /// <inheritdoc/>
    public string Visit(IIRPrinterContext context, Require target, bool ILmode)
    {
        var condition = context.GetArgument(target, Require.Predicate);
        var value = context.GetArgument(target, Require.Value);
        return $"IR.F.Math.Require({condition}, {value})";
    }
}
