// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Nncase.IR;

namespace Nncase.Transform.Pattern
{
    public sealed record TuplePattern(IRArray<ExprPattern> Fields) : ExprPattern
    {
        public TuplePattern(IR.Tuple tuple) : this(ImmutableArray.Create((from f in tuple.Fields select (ExprPattern)f).ToArray())) { }

        public bool MatchLeaf(IR.Tuple tuple)
        {
            return (Fields.Count == tuple.Fields.Count) && MatchCheckedType(tuple);
        }

    }
    public static partial class Functional
    {
        public static TuplePattern IsTuple(ExprPattern[] Fields) => new TuplePattern(Fields);
    }
}