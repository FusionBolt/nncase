// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.
using System;
using Nncase.PatternMatch;

namespace Nncase.IR.Math;

/// <summary>
/// Quantize expression.
/// </summary>
[PatternFunctionalGenerator]
public sealed record Quantize(DataType TargetType) : Op
{
    /// <summary>
    /// Gets input.
    /// </summary>
    public static readonly ParameterInfo Input = new(typeof(Quantize), 0, "input");

    /// <summary>
    /// Gets QuantParam.
    /// </summary>
    public static readonly ParameterInfo QuantParam = new(typeof(Quantize), 1, "quantParam");
}
