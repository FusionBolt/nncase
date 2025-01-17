// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Nncase.Evaluator;
using Nncase.IR;
using Nncase.PatternMatch;
using static Nncase.PatternMatch.Utility;
using static Nncase.Transform.RulesFactory;

namespace Nncase.Transform.Rules.Neutral;

public static partial class SimplifyFactory
{
    private static ExprPattern x = IsWildcard(), y = IsWildcard(), z = IsWildcard(), w = IsWildcard(), u = IsWildcard(), v = IsWildcard();
    private static ConstPattern c0 = IsConst(), c1 = IsConst(), c2 = IsConst(), c3 = IsConst(), c4 = IsConst(), c5 = IsConst();

    private static readonly List<IRewriteRule> _simplifyAdd = new()
    {
        Rewrite(x + 0, x),
        Rewrite(0 + x, x),
        Rewrite(x + c0 + c1, x + (c0 + c1)),
        Rewrite(c0 + x + c1, x + (c0 + c1)),
        Rewrite(c1 + (x + c0), x + (c0 + c1)),
        Rewrite(c1 + (c0 + x), x + (c0 + c1)),
    };

    public static List<IRewriteRule> SimplifyAdd()
    {
        return _simplifyAdd;
    }

    private static readonly List<IRewriteRule> _simplifyMul = new()
    {
        Rewrite(x * 1, x),
        Rewrite(1 * x, x),
    };

    public static List<IRewriteRule> SimplifyMul()
    {
        return _simplifyMul;
    }
}
