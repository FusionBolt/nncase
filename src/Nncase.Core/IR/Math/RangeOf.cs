// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.
using System;
using Nncase.PatternMatch;

namespace Nncase.IR.Math;

/// <summary>
/// RangeOf expression.
/// </summary>
[PatternFunctionalGenerator]
public sealed record RangeOf() : Op
{
    /// <summary>
    /// Gets input.
    /// </summary>
    public static readonly ParameterInfo Input = new(typeof(RangeOf), 0, "input");
}