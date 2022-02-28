using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nncase.IR;
using Nncase.IR.Math;
using Nncase.PatternMatch;
using Nncase.Transform;
using Xunit;
using static Nncase.IR.F.Math;
using static Nncase.IR.F.Tensors;
using static Nncase.IR.TypePatternUtility;
using static Nncase.PatternMatch.F.Math;
using static Nncase.PatternMatch.F.Tensors;
using static Nncase.PatternMatch.Utility;

namespace Nncase.Tests.CoreTest;


public class UnitTestExprPattern
{

    [Fact]
    public void TestPatternEqual()
    {
        var p1 = IsWildcard();
        int p1hash = p1.GetHashCode();
        var p2 = IsBinary(b => true, IsConst(), IsConst());

        ExprPattern p3 = p1 with { TypePattern = IsScalar() };
        int p3hash = p3.GetHashCode();
        Assert.Equal(p1hash, p3hash);
    }

    [Fact]
    public void TestVarPattern()
    {
        Var e = new Var("x", AnyType.Default);
        Assert.False(e.InferenceType());
        ExprPattern ep = new ExprPattern(expr => expr == e, null);
        Assert.IsType<VarPattern>(ep);
        Assert.True(ep.MatchLeaf(e));
    }

    [Fact]
    public void TestTensorConstantPattern()
    {
        var con = (Const)(1.1f);
        Assert.True(con.InferenceType());
        ExprPattern cp1 = new ExprPattern(e => e == con, null);
        Assert.IsType<TensorConstPattern>(cp1);

        var cp2 = IsConst((float x) => x > 1.2f);
        var cp3 = IsConst((int x) => x > 1);
        var cp4 = (TensorConstPattern)1.1f;

        Assert.True(cp1.MatchLeaf(con));
        Assert.False(cp2.MatchLeaf(con));
        Assert.False(cp3.MatchLeaf(con));
        Assert.True(cp4.MatchLeaf(con));
    }

    [Fact]
    public void TestTensorConstantPatternEqual()
    {
        TensorConstPattern cp1 = (TensorConstPattern)1;
        TensorConstPattern cp2 = (TensorConstPattern)1;
        Dictionary<TensorConstPattern, int> d = new();
        d.Add(cp1, 1);
        Assert.NotEqual(cp1, cp2);
        Assert.DoesNotContain(cp2, d.Keys);
        TensorConstPattern cp3 = IsTensorConst();
        TensorConstPattern cp4 = IsTensorConst();
        d.Add(cp3, 1);
        Assert.NotEqual(cp3, cp4);
        Assert.DoesNotContain(cp4, d.Keys);
    }

    [Fact]
    public void TestWildcardPattern()
    {
        var wc = IsWildcard();
        Assert.IsType<ExprPattern>(wc);
    }

    [Fact]
    public void TestWildCardPatternHash()
    {
        var wc = IsWildcard();
        var wc2 = new ExprPattern(x => true, null);
        var wc3 = new ExprPattern(x => true, null);
        var d = new Dictionary<ExprPattern, int>();
        d.Add(wc, 1);
        d.Add(wc2, 2);
        d.Add(wc3, 2);
    }

    [Fact]
    public void TestCallPattern()
    {
        var e = (Const)1 + Exp(10);
        Assert.True(e.InferenceType());
        var wc1 = IsWildcard();
        var wc2 = IsWildcard();
        var c = wc1 + wc2;
        Assert.IsType<CallPattern>(c);
        Assert.IsType<OpPattern<Binary>>(c.Target);
        Assert.IsType<ExprPattern>(c.Parameters[0]);
        Assert.IsType<ExprPattern>(c.Parameters[1]);

        CallPattern c2 = IsBinary(BinaryOp.Add, wc1, wc2);

        CallPattern c3 = IsBinary(x => x.BinaryOp is (BinaryOp.Div or BinaryOp.Sub), wc1, wc2);

        Assert.True(c.Target.MatchLeaf(e.Target));
        Assert.True(c2.Target.MatchLeaf(e.Target));
        Assert.False(c3.Target.MatchLeaf(e.Target));
    }

    [Fact]
    public void TestFunctionPattern()
    {
        var wc1 = IsWildcard();
        var wc2 = IsWildcard();
        var c = wc1 + wc2;
        var fp = new FunctionPattern(c, new[] { wc1, wc2 }, null);
        Assert.IsType<FunctionPattern>(fp);
        Assert.IsType<ExprPattern>(fp.Parameters[0]);
        Assert.IsType<ExprPattern>(fp.Parameters[1]);
        Assert.IsType<CallPattern>(fp.Body);
        Assert.IsType<ExprPattern>(((CallPattern)fp.Body).Parameters[0]);
        Assert.IsType<ExprPattern>(((CallPattern)fp.Body).Parameters[1]);

        var fp2 = new FunctionPattern(c, IsVArgs(new[] { wc1, wc2 }), null);
        Assert.IsType<ExprPattern>(fp.Parameters[0]);
        Assert.IsType<ExprPattern>(fp.Parameters[1]);
    }

    [Fact]
    public void TestTuplePattern()
    {
        var wc1 = IsWildcard();
        var wc2 = IsWildcard();
        var t = IsTuple(new[] { wc1, wc2 });
        Assert.IsType<TuplePattern>(t);
        Assert.IsType<ExprPattern>(t.Fields[0]);
        Assert.IsType<ExprPattern>(t.Fields[1]);

        var t2 = IsTuple(IsVArgs(new[] { wc1, wc2 }));
        Assert.IsType<TuplePattern>(t2);
        Assert.IsType<ExprPattern>(t2.Fields[0]);
        Assert.IsType<ExprPattern>(t2.Fields[1]);
    }

    [Fact]
    public void TestVArgsPattern()
    {
        // var wc = IsWildcard();
        // var vwcs = new List<ExprPattern>();
        // var pattern = IsVArgsRepeat((n, param) =>
        // {
        //     for (int i = 0; i < n; i++)
        //     {
        //         var wc = IsWildcard();
        //         param.Add(wc);
        //         vwcs.Add(wc);
        //     }
        // },
        // (match, param) =>
        // {
        //     if (match == false)
        //     {
        //         param.Clear();
        //         vwcs.Clear();
        //     }
        // }
        // );

        // var tuple = new IR.Tuple(1, new IR.Tuple(6, 7, 8), 3, 4);
        // tuple.InferenceType();
        // Assert.True(pattern.MatchLeaf(tuple.Fields));
        // Assert.True(pattern[0].MatchLeaf(tuple[0]));
        // Assert.True(pattern[1].MatchLeaf(tuple[1]));
        // Assert.True(pattern[2].MatchLeaf(tuple[2]));
        // Assert.True(pattern[3].MatchLeaf(tuple[3]));
    }

    [Fact]
    public void TestVArgsPatternFunc()
    {
        var pat = IsVArgsRepeat(() => IsConst());
        IR.Tuple expr = new IR.Tuple(1, 2, 3, 4, 5, 6);
        pat.MatchLeaf(expr.Fields);
        Assert.Equal(pat.Count, expr.Fields.Count);
    }

    [Fact]
    public void TestAltPattern()
    {
        var lhs = IsWildcard();
        var rhs = IsWildcard();
        var is_op_call = IsCall(IsWildcard(), new[] { lhs, rhs });
        Const x = (Const)1;
        Const y = (Const)2;
        var z1 = x + y;
        var z2 = x * y;
        z1.InferenceType();
        z2.InferenceType();
        Assert.True(is_op_call.MatchLeaf(z1));
        Assert.True(is_op_call.Target.MatchLeaf(z2.Target));

        var is_op_call2 = IsCall(IsWildcard(), IsVArgs(new[] { lhs, rhs }));

        Assert.IsType<ExprPattern>(is_op_call2.Parameters[0]);
        Assert.IsType<ExprPattern>(is_op_call2.Parameters[1]);
    }

    [Fact]
    public void TestTypePattern()
    {
        var ttype = new TensorType(DataTypes.Float32, new[] { 10, 10 });
        var ty_pat = IsType(ttype);
        Assert.IsType<TypePattern>(ty_pat);
        Assert.True(ty_pat.MatchLeaf(ttype));
    }

    [Fact]
    public void TestDataTypePattern()
    {
        var ttype1 = new TensorType(DataTypes.Float32, new[] { 10, 10 });
        var ttype2 = new TensorType(DataTypes.Int16, new[] { 10 });
        var ty_pat = IsDataType(DataTypes.Float32);
        Assert.IsType<TypePattern>(ty_pat);
        Assert.True(ty_pat.MatchLeaf(ttype1));
        Assert.False(ty_pat.MatchLeaf(ttype2));
    }

    [Fact]
    public void TestShapePattern()
    {
        var shape = new int[] { 10, 10 };
        var sp = IsShape(shape);
        var ttype1 = new TensorType(DataTypes.Float32, new[] { 10, 10 });
        var ttype2 = new TensorType(DataTypes.Int16, new[] { 10 });
        Assert.True(sp.MatchLeaf(ttype1));
        Assert.False(sp.MatchLeaf(ttype2));
    }

    [Fact]
    public void TestPatternClone()
    {
        var pat = IsWildcard();
        var pat2 = IsWildcard();
        Assert.NotEqual(pat, pat2);
    }

    [Fact]
    public void TestBuildExprFromPattern()
    {
        ConstPattern c0 = IsConst(), c1 = IsConst();
        var x = IsWildcard();
        var pat = x + c0;
        var res = x - c0;
        var ped = c0 == 0;
    }
}