// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.
using System;
using Nncase.PatternMatch;

namespace Nncase.IR.Math;

/// <summary>
/// Dequantize expression.
/// </summary>
[PatternFunctionalGenerator]
public sealed record Dequantize(DataType TargetType) : Op
{
    /// <summary>
    /// Gets input.
    /// </summary>
    public static readonly ParameterInfo Input = new(typeof(Dequantize), 0, "input");

    /// <summary>
    /// Gets DequantParam.
    /// </summary>
    public static readonly ParameterInfo DequantParam = new(typeof(Dequantize), 1, "dequantParam");
}
