﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nncase.IR;

/// <summary>
/// Tuple interface.
/// </summary>
public interface ITuple : IReadOnlyList<Expr>
{
    /// <summary>
    /// Gets fields.
    /// </summary>
    IReadOnlyList<Expr> Fields { get; }

    /// <summary>
    /// Cast to expression.
    /// </summary>
    /// <returns>The expression.</returns>
    public Expr AsExpr() => (Expr)this;
}
