﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nncase.IR;

namespace Nncase.PatternMatch;

/// <summary>
/// mark the record class auto generate the pattern define.
/// </summary>
public class PatternFunctionalGeneratorAttribute : Attribute { }

/// <summary>
/// Pattern.
/// </summary>
public interface IPattern
{
    /// <summary>
    /// get the name of this pattern.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Match leaf.
    /// </summary>
    /// <param name="input">Input.</param>
    /// <returns>Is match.</returns>
    bool MatchLeaf(object input);
}

/// <summary>
/// Pattern.
/// </summary>
/// <typeparam name="TInput">Input type.</typeparam>
public interface IPattern<in TInput> : IPattern
{
    /// <summary>
    /// Match leaf.
    /// </summary>
    /// <param name="input">Input.</param>
    /// <returns>Is match.</returns>
    bool MatchLeaf(TInput input);
}
