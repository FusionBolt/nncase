// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.IO;
using Nncase.IR;
using F = Nncase.IR.F;

namespace Nncase.Importer.TFLite
{
    public partial class TFLiteImporter
    {
        private Expr VisitOneHot(in tflite.Operator op)
        {
            var (indices, depth) = GetInputExprs(op, 0, 1);
            var (onValue, offValue) = GetInputExprs(op, 2, 3);
            return F.NN.OneHot(OneHotMode.ProcessNeg, indices, depth, 
                F.Tensors.Concat(new Tuple(offValue, onValue), 0),
                op.BuiltinOptionsAsOneHotOptions().Axis);
        }
    }
}