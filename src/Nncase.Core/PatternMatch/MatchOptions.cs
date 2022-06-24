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
/// Match options.
/// </summary>
public class MatchOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchOptions"/> class.
    /// </summary>
    public MatchOptions()
    {
        SuppressedPatterns = new Dictionary<Expr, HashSet<IPattern>>(ReferenceEqualityComparer.Instance);
    }

    /// <summary>
    /// Gets suppressed patterns.
    /// </summary>
    public Dictionary<Expr, HashSet<IPattern>> SuppressedPatterns { get; }

    public bool IsSuppressedPattern(Expr expr, IPattern pattern)
    {
        if (SuppressedPatterns.TryGetValue(expr, out var patterns))
        {
            return patterns.Contains(pattern);
        }

        return false;
    }

    public void SuppressPattern(Expr expr, IPattern pattern)
    {
        if (!SuppressedPatterns.TryGetValue(expr, out var patterns))
        {
            patterns = new HashSet<IPattern>();
            SuppressedPatterns.Add(expr, patterns);
        }

        patterns.Add(pattern);
    }

    public void InheritSuppressPatterns(Expr source, Expr dest)
    {
        if (SuppressedPatterns.TryGetValue(source, out var srcPatterns))
        {
            if (!SuppressedPatterns.TryGetValue(dest, out var destPatterns))
            {
                destPatterns = new HashSet<IPattern>(srcPatterns);
                SuppressedPatterns.Add(dest, destPatterns);
            }
            else
            {
                foreach (var pattern in srcPatterns)
                {
                    destPatterns.Add(pattern);
                }
            }
        }
    }
}