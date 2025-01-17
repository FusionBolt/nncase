﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nncase.IR;
using Nncase.IR.F;
using Nncase.Transform;
using Nncase.Transform.Rules.Neutral;
using Xunit;
using Math = Nncase.IR.F.Math;
using Random = Nncase.IR.F.Random;

namespace Nncase.Tests.Rules.NeutralTest;

public class UnitTestFusePadConv2D : TestFixture.UnitTestFixtrue
{
    public static IEnumerable<object[]> TestFusePadConv2DPositiveData =>
        new[]
        {
            new object[] { new[] { 1, 1, 2, 2 }, new[,] { { 1, 0 },{ 0, 0 },{ 3, 3 },{ 4, 4 } }, new[,] { { 0, 0 }, { 0, 0 } }, new[] { 3, 1, 1, 1 } }, // fuse hw pad, keep n pad 
            new object[] { new[] { 1, 3, 4, 1 }, new[,] { { 0, 0 },{ 0, 0 },{ 5, 0 },{ 1, 3 } }, new[,] { { 0, 2 }, { 3, 2 } }, new[] { 1, 3, 2, 2 } }, // fuse hw pad
            new object[] { new[] { 1, 3, 4, 2 }, new[,] { { 0, 0 },{ 0, 0 },{ 0, 0 },{ 0, 0 } }, new[,] { { 0, 2 }, { 3, 2 } }, new[] { 1, 3, 2, 2 } }, // nop pad
        }.Select((o, i) => o.Concat(new object[] { i }).ToArray());

    [Theory]
    [MemberData(nameof(TestFusePadConv2DPositiveData))]
    public void TestFusePadConv2DPositive(int[] shape, int[,] pads1, int[,] pads2, int[] wShape, int index)
    {
        var caseOptions = GetPassOptions();
        var a = new Var();
        var w = Random.Normal(DataTypes.Float32, 0, 1, 0, wShape);
        var b = Random.Normal(DataTypes.Float32, 0, 1, 0, new[] { wShape[0] });

        var ANormal = new Dictionary<Var, IValue>();
        ANormal.Add(a, Random.Normal(DataTypes.Float32, 0, 1, 0, shape).Evaluate());
        var rootPre = NN.Conv2D(NN.Pad(a, pads1, PadMode.Constant, 0f), w, b, new[] { 1, 1 }, pads2, new[] { 1, 1 }, PadMode.Constant, 1);
        var rootMid = CompilerServices.Rewrite(rootPre, new IRewriteRule[]
        {
            new FoldConstCall(),
            // new FusePadConv2d(),
            // new FoldNopPad(),
            
        }, caseOptions);
        var rootPost = CompilerServices.Rewrite(rootMid, new IRewriteRule[]
        {
            // new FoldConstCall(),
            new FusePadConv2d(),
            new FoldConstCall(),
            new FoldNopPad(),

        }, caseOptions);


        Assert.NotEqual(rootMid, rootPost);
        Assert.Equal(CompilerServices.Evaluate(rootMid, ANormal), CompilerServices.Evaluate(rootPost, ANormal));
    }

    public static IEnumerable<object[]> TestFusePadConv2DNegativeData =>
        new[]
        {
            new object[] { new[] { 1, 3, 4, 2 }, new[,] { { 1, 0 },{ 0, 0 },{ 0, 0 },{ 0, 0 } }, new[,] { { 0, 2 }, { 3, 2 } }, new[] { 2, 3, 2, 2 } }, // can't fuse n pad
            new object[] { new[] { 1, 3, 4, 2 }, new[,] { { 0, 0 },{ 0, 1 },{ 0, 0 },{ 0, 0 } }, new[,] { { 0, 2 }, { 3, 2 } }, new[] { 1, 4, 2, 2 } }, // can't fuse c pad
        }.Select((o, i) => o.Concat(new object[] { i }).ToArray());

    [Theory]
    [MemberData(nameof(TestFusePadConv2DNegativeData))]
    public void TestFusePadConv2DNegative(int[] shape, int[,] pads1, int[,] pads2, int[] wShape, int index)
    {
        var caseOptions = GetPassOptions();
        var a = new Var();
        var w = Random.Normal(DataTypes.Float32, 0, 1, 0, wShape);
        var b = Random.Normal(DataTypes.Float32, 0, 1, 0, new[] { wShape[0] });

        var ANormal = new Dictionary<Var, IValue>();
        ANormal.Add(a, Random.Normal(DataTypes.Float32, 0, 1, 0, shape).Evaluate());
        var rootPre = NN.Conv2D(NN.Pad(a, pads1, PadMode.Constant, 0f), w, b, new[] { 1, 1 }, pads2, new[] { 1, 1 }, PadMode.Constant, 1);

        var rootMid = CompilerServices.Rewrite(rootPre, new IRewriteRule[]
        {
            new FoldConstCall(),
        }, caseOptions);

        var rootPost = CompilerServices.Rewrite(rootMid, new IRewriteRule[]
        {
            new FusePadConv2d(),
            new FoldNopPad(),
        }, caseOptions);


        Assert.Equal(rootMid, rootPost);
        Assert.Equal(CompilerServices.Evaluate(rootMid, ANormal), CompilerServices.Evaluate(rootPost, ANormal));
    }
}
