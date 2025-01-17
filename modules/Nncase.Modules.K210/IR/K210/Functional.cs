﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nncase.IR.K210;
using Nncase.IR.NN;
using Nncase.IR.Random;
using Nncase.IR.Tensors;

namespace Nncase.IR.F;

/// <summary>
/// K210 functional helper.
/// </summary>
public static class K210
{
    public static Call FakeKPUConv2D(bool isDepthwise, KPUFilterType filterType, KPUPoolType poolType, Tensor<float> bias, ValueRange<float> fusedClamp, Expr input, Expr weights) =>
        new Call(new FakeKPUConv2D(isDepthwise, filterType, poolType, bias, fusedClamp), input, weights);

    public static Call FakeKPUUpload(Expr input) =>
        new Call(new FakeKPUUpload(), input);

    public static Call FakeKPUDownload(Expr input) =>
        new Call(new FakeKPUDownload(), input);
}
