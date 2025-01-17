﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nncase.IR.Buffer;

namespace Nncase.IR.F;

/// <summary>
/// Random functional helper.
/// </summary>
public static class Buffer
{
    /// <summary>
    /// the placeholder for this expr's ddr pointer.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Call DDrOf(Expr input) =>
        new Call(new DDrOf(), input);

    /// <summary>
    /// the placeholder for the expr's basement value.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Call BaseMentOf(Expr input) =>
        new Call(new BaseMentOf(), input);

    /// <summary>
    /// the placeholder for the expr's strides
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Call StrideOf(Expr input) => new Call(new StrideOf(), input);
}
