// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Nncase.IR;

namespace Nncase.PatternMatch;

/// <summary>
/// Pattern for <see cref="Const"/>.
/// </summary>
/// <param name="Fields">Fields condition.</param>
/// <param name="Name">name.</param>
public sealed record TuplePattern(VArgsPattern Fields, string? Name) : Pattern<IR.Tuple>(Name)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TuplePattern"/> class.
    /// </summary>
    /// <param name="tuple"><see cref="IR.Tuple"/> expression.</param>
    /// <param name="name">name.</param>
    public TuplePattern(IR.Tuple tuple, string? name)
        : this(new VArgsPattern(tuple.Fields, null), name)
    {
    }
}

public static partial class Utility
{
    /// <summary>
    /// Create tuple pattern.
    /// </summary>
    /// <param name="fields">fields.</param>
    /// <param name="name">name.</param>
    /// <returns>TuplePattern .</returns>
    public static TuplePattern IsTuple(Pattern[] fields, string? name = null) => new TuplePattern(new VArgsPattern(fields, null), name);

    /// <summary>
    /// Create tuple pattern.
    /// </summary>
    /// <param name="fields">fields.</param>
    /// <param name="name">name.</param>
    /// <returns>TuplePattern .</returns>
    public static TuplePattern IsTuple(VArgsPattern fields, string? name = null) => new TuplePattern(fields, name);

    /// <summary>
    /// Create tuple pattern.
    /// </summary>
    /// <param name="name">name.</param>
    /// <returns>TuplePattern .</returns>
    public static TuplePattern IsTuple(string? name) => IsTuple(IsVArgsRepeat(() => IsWildcard()), name);

    /// <summary>
    /// Create tuple pattern.
    /// </summary>
    /// <returns>TuplePattern .</returns>
    public static TuplePattern IsConstTuple() => IsTuple(IsVArgsRepeat(() => IsConst()));
}
