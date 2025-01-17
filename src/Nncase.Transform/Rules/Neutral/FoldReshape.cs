// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Nncase.IR;
using Nncase.IR.Math;
using Nncase.IR.Tensors;
using Nncase.PatternMatch;
using static Nncase.IR.F.Math;
using static Nncase.IR.F.Tensors;
using static Nncase.IR.TypePatternUtility;
using static Nncase.PatternMatch.F.Math;
using static Nncase.PatternMatch.F.Tensors;
using static Nncase.PatternMatch.Utility;

namespace Nncase.Transform.Rules.Neutral;

/// <summary>
/// Fold nop <see cref="IR.Tensors.Reshape"/>.
/// </summary>
[RuleGenerator]
public sealed partial class FoldNopReshape : IRewriteRule
{
    /// <inheritdoc/>
    public IPattern Pattern { get; } = IsReshape(
        IsWildcard("input") with { TypePattern = HasFixedShape() },
        IsTensorConst("newShape", IsIntegral()));

    private Expr? GetReplace(Expr input, TensorConst newShape)
    {
        if (input.CheckedShape.Equals(newShape.Value.ToArray<int>()))
        {
            return input;
        }

        return null;
    }
}

/// <summary>
/// Fold two <see cref="IR.Tensors.Reshape"/>.
/// </summary>
[RuleGenerator]
public sealed partial class FoldTwoReshapes : IRewriteRule
{
    /// <inheritdoc/>
    public IPattern Pattern { get; } = IsReshape(
        IsReshape(IsWildcard("input"), IsWildcard()), IsWildcard("newShape"));

    private Expr? GetReplace(Expr input, Expr newShape)
    {
        return Reshape(input, newShape);
    }
}
