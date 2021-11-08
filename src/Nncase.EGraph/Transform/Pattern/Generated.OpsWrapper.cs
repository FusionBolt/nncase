using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nncase.IR;
using Nncase.IR.Math;
using Nncase.IR.NN;
using Nncase.IR.Tensors;

namespace Nncase.Transform.Pattern.Math
{
    public sealed record BinaryWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern LhsPat() => Pattern[Binary.Lhs];
        public T LhsPat<T>()
            where T : ExprPattern => (T)LhsPat();
        public Expr Lhs() => GetCast<Expr>(LhsPat());
        public T Lhs<T>()
            where T : Expr => GetCast<T>(LhsPat());
        public ExprPattern RhsPat() => Pattern[Binary.Rhs];
        public T RhsPat<T>()
            where T : ExprPattern => (T)RhsPat();
        public Expr Rhs() => GetCast<Expr>(RhsPat());
        public T Rhs<T>()
            where T : Expr => GetCast<T>(RhsPat());
        public BinaryOp BinaryOp => ((Binary)GetCast<Call>(this).Target).BinaryOp;
        public static implicit operator CallPattern(BinaryWrapper warper) => warper.Pattern;
    }

    public sealed record ClampWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Clamp.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern MinPat() => Pattern[Clamp.Min];
        public T MinPat<T>()
            where T : ExprPattern => (T)MinPat();
        public Expr Min() => GetCast<Expr>(MinPat());
        public T Min<T>()
            where T : Expr => GetCast<T>(MinPat());
        public ExprPattern MaxPat() => Pattern[Clamp.Max];
        public T MaxPat<T>()
            where T : ExprPattern => (T)MaxPat();
        public Expr Max() => GetCast<Expr>(MaxPat());
        public T Max<T>()
            where T : Expr => GetCast<T>(MaxPat());
        public static implicit operator CallPattern(ClampWrapper warper) => warper.Pattern;
    }

    public sealed record UnaryWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Unary.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public UnaryOp UnaryOp => ((Unary)GetCast<Call>(this).Target).UnaryOp;
        public static implicit operator CallPattern(UnaryWrapper warper) => warper.Pattern;
    }
}

namespace Nncase.Transform.Pattern.NN
{
    public sealed record SigmoidWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Sigmoid.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public static implicit operator CallPattern(SigmoidWrapper warper) => warper.Pattern;
    }

    public sealed record Conv2DWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Conv2D.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern WeightsPat() => Pattern[Conv2D.Weights];
        public T WeightsPat<T>()
            where T : ExprPattern => (T)WeightsPat();
        public Expr Weights() => GetCast<Expr>(WeightsPat());
        public T Weights<T>()
            where T : Expr => GetCast<T>(WeightsPat());
        public ExprPattern BiasPat() => Pattern[Conv2D.Bias];
        public T BiasPat<T>()
            where T : ExprPattern => (T)BiasPat();
        public Expr Bias() => GetCast<Expr>(BiasPat());
        public T Bias<T>()
            where T : Expr => GetCast<T>(BiasPat());
        public ExprPattern StridePat() => Pattern[Conv2D.Stride];
        public T StridePat<T>()
            where T : ExprPattern => (T)StridePat();
        public Expr Stride() => GetCast<Expr>(StridePat());
        public T Stride<T>()
            where T : Expr => GetCast<T>(StridePat());
        public ExprPattern PaddingPat() => Pattern[Conv2D.Padding];
        public T PaddingPat<T>()
            where T : ExprPattern => (T)PaddingPat();
        public Expr Padding() => GetCast<Expr>(PaddingPat());
        public T Padding<T>()
            where T : Expr => GetCast<T>(PaddingPat());
        public ExprPattern DilationPat() => Pattern[Conv2D.Dilation];
        public T DilationPat<T>()
            where T : ExprPattern => (T)DilationPat();
        public Expr Dilation() => GetCast<Expr>(DilationPat());
        public T Dilation<T>()
            where T : Expr => GetCast<T>(DilationPat());
        public ExprPattern GroupsPat() => Pattern[Conv2D.Groups];
        public T GroupsPat<T>()
            where T : ExprPattern => (T)GroupsPat();
        public Expr Groups() => GetCast<Expr>(GroupsPat());
        public T Groups<T>()
            where T : Expr => GetCast<T>(GroupsPat());
        public PadMode padMode => ((Conv2D)GetCast<Call>(this).Target).padMode;
        public static implicit operator CallPattern(Conv2DWrapper warper) => warper.Pattern;
    }
}

namespace Nncase.Transform.Pattern.Tensors
{
    public sealed record BatchToSpaceWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[BatchToSpace.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern BlockShapePat() => Pattern[BatchToSpace.BlockShape];
        public T BlockShapePat<T>()
            where T : ExprPattern => (T)BlockShapePat();
        public Expr BlockShape() => GetCast<Expr>(BlockShapePat());
        public T BlockShape<T>()
            where T : Expr => GetCast<T>(BlockShapePat());
        public ExprPattern CropsPat() => Pattern[BatchToSpace.Crops];
        public T CropsPat<T>()
            where T : ExprPattern => (T)CropsPat();
        public Expr Crops() => GetCast<Expr>(CropsPat());
        public T Crops<T>()
            where T : Expr => GetCast<T>(CropsPat());
        public static implicit operator CallPattern(BatchToSpaceWrapper warper) => warper.Pattern;
    }

    public sealed record BroadcastWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Broadcast.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern ShapePat() => Pattern[Broadcast.Shape];
        public T ShapePat<T>()
            where T : ExprPattern => (T)ShapePat();
        public Expr Shape() => GetCast<Expr>(ShapePat());
        public T Shape<T>()
            where T : Expr => GetCast<T>(ShapePat());
        public static implicit operator CallPattern(BroadcastWrapper warper) => warper.Pattern;
    }

    public sealed record CastWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Cast.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public DataType NewType => ((Cast)GetCast<Call>(this).Target).NewType;
        public static implicit operator CallPattern(CastWrapper warper) => warper.Pattern;
    }

    public sealed record ConcatWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Concat.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern AxisPat() => Pattern[Concat.Axis];
        public T AxisPat<T>()
            where T : ExprPattern => (T)AxisPat();
        public Expr Axis() => GetCast<Expr>(AxisPat());
        public T Axis<T>()
            where T : Expr => GetCast<T>(AxisPat());
        public static implicit operator CallPattern(ConcatWrapper warper) => warper.Pattern;
    }

    public sealed record DeQuantizeWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[DeQuantize.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern QuantParamPat() => Pattern[DeQuantize.QuantParam];
        public T QuantParamPat<T>()
            where T : ExprPattern => (T)QuantParamPat();
        public Expr QuantParam() => GetCast<Expr>(QuantParamPat());
        public T QuantParam<T>()
            where T : Expr => GetCast<T>(QuantParamPat());
        public DataType TargetType => ((DeQuantize)GetCast<Call>(this).Target).TargetType;
        public static implicit operator CallPattern(DeQuantizeWrapper warper) => warper.Pattern;
    }

    public sealed record GatherWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Gather.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern AxisPat() => Pattern[Gather.Axis];
        public T AxisPat<T>()
            where T : ExprPattern => (T)AxisPat();
        public Expr Axis() => GetCast<Expr>(AxisPat());
        public T Axis<T>()
            where T : Expr => GetCast<T>(AxisPat());
        public ExprPattern IndexPat() => Pattern[Gather.Index];
        public T IndexPat<T>()
            where T : ExprPattern => (T)IndexPat();
        public Expr Index() => GetCast<Expr>(IndexPat());
        public T Index<T>()
            where T : Expr => GetCast<T>(IndexPat());
        public static implicit operator CallPattern(GatherWrapper warper) => warper.Pattern;
    }

    public sealed record GatherNDWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[GatherND.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern AxisPat() => Pattern[GatherND.Axis];
        public T AxisPat<T>()
            where T : ExprPattern => (T)AxisPat();
        public Expr Axis() => GetCast<Expr>(AxisPat());
        public T Axis<T>()
            where T : Expr => GetCast<T>(AxisPat());
        public ExprPattern BatchDimsPat() => Pattern[GatherND.BatchDims];
        public T BatchDimsPat<T>()
            where T : ExprPattern => (T)BatchDimsPat();
        public Expr BatchDims() => GetCast<Expr>(BatchDimsPat());
        public T BatchDims<T>()
            where T : Expr => GetCast<T>(BatchDimsPat());
        public ExprPattern IndexPat() => Pattern[GatherND.Index];
        public T IndexPat<T>()
            where T : ExprPattern => (T)IndexPat();
        public Expr Index() => GetCast<Expr>(IndexPat());
        public T Index<T>()
            where T : Expr => GetCast<T>(IndexPat());
        public static implicit operator CallPattern(GatherNDWrapper warper) => warper.Pattern;
    }

    public sealed record MatMulWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[MatMul.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern OtherPat() => Pattern[MatMul.Other];
        public T OtherPat<T>()
            where T : ExprPattern => (T)OtherPat();
        public Expr Other() => GetCast<Expr>(OtherPat());
        public T Other<T>()
            where T : Expr => GetCast<T>(OtherPat());
        public static implicit operator CallPattern(MatMulWrapper warper) => warper.Pattern;
    }

    public sealed record PadWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Pad.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern PadsPat() => Pattern[Pad.Pads];
        public T PadsPat<T>()
            where T : ExprPattern => (T)PadsPat();
        public Expr Pads() => GetCast<Expr>(PadsPat());
        public T Pads<T>()
            where T : Expr => GetCast<T>(PadsPat());
        public ExprPattern ValuePat() => Pattern[Pad.Value];
        public T ValuePat<T>()
            where T : ExprPattern => (T)ValuePat();
        public Expr Value() => GetCast<Expr>(ValuePat());
        public T Value<T>()
            where T : Expr => GetCast<T>(ValuePat());
        public PadMode padMode => ((Pad)GetCast<Call>(this).Target).padMode;
        public static implicit operator CallPattern(PadWrapper warper) => warper.Pattern;
    }

    public sealed record QuantizeWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Quantize.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern QuantParamPat() => Pattern[Quantize.QuantParam];
        public T QuantParamPat<T>()
            where T : ExprPattern => (T)QuantParamPat();
        public Expr QuantParam() => GetCast<Expr>(QuantParamPat());
        public T QuantParam<T>()
            where T : Expr => GetCast<T>(QuantParamPat());
        public DataType TargetType => ((Quantize)GetCast<Call>(this).Target).TargetType;
        public static implicit operator CallPattern(QuantizeWrapper warper) => warper.Pattern;
    }

    public sealed record ReduceWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Reduce.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern AxisPat() => Pattern[Reduce.Axis];
        public T AxisPat<T>()
            where T : ExprPattern => (T)AxisPat();
        public Expr Axis() => GetCast<Expr>(AxisPat());
        public T Axis<T>()
            where T : Expr => GetCast<T>(AxisPat());
        public ExprPattern InitValuePat() => Pattern[Reduce.InitValue];
        public T InitValuePat<T>()
            where T : ExprPattern => (T)InitValuePat();
        public Expr InitValue() => GetCast<Expr>(InitValuePat());
        public T InitValue<T>()
            where T : Expr => GetCast<T>(InitValuePat());
        public ExprPattern KeepDimsPat() => Pattern[Reduce.KeepDims];
        public T KeepDimsPat<T>()
            where T : ExprPattern => (T)KeepDimsPat();
        public Expr KeepDims() => GetCast<Expr>(KeepDimsPat());
        public T KeepDims<T>()
            where T : Expr => GetCast<T>(KeepDimsPat());
        public ReduceOp reduceOp => ((Reduce)GetCast<Call>(this).Target).reduceOp;
        public static implicit operator CallPattern(ReduceWrapper warper) => warper.Pattern;
    }

    public sealed record ReshapeWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Reshape.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern ShapePat() => Pattern[Reshape.Shape];
        public T ShapePat<T>()
            where T : ExprPattern => (T)ShapePat();
        public Expr Shape() => GetCast<Expr>(ShapePat());
        public T Shape<T>()
            where T : Expr => GetCast<T>(ShapePat());
        public static implicit operator CallPattern(ReshapeWrapper warper) => warper.Pattern;
    }

    public sealed record SliceWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Slice.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern BeginsPat() => Pattern[Slice.Begins];
        public T BeginsPat<T>()
            where T : ExprPattern => (T)BeginsPat();
        public Expr Begins() => GetCast<Expr>(BeginsPat());
        public T Begins<T>()
            where T : Expr => GetCast<T>(BeginsPat());
        public ExprPattern EndsPat() => Pattern[Slice.Ends];
        public T EndsPat<T>()
            where T : ExprPattern => (T)EndsPat();
        public Expr Ends() => GetCast<Expr>(EndsPat());
        public T Ends<T>()
            where T : Expr => GetCast<T>(EndsPat());
        public ExprPattern AxesPat() => Pattern[Slice.Axes];
        public T AxesPat<T>()
            where T : ExprPattern => (T)AxesPat();
        public Expr Axes() => GetCast<Expr>(AxesPat());
        public T Axes<T>()
            where T : Expr => GetCast<T>(AxesPat());
        public ExprPattern StridesPat() => Pattern[Slice.Strides];
        public T StridesPat<T>()
            where T : ExprPattern => (T)StridesPat();
        public Expr Strides() => GetCast<Expr>(StridesPat());
        public T Strides<T>()
            where T : Expr => GetCast<T>(StridesPat());
        public static implicit operator CallPattern(SliceWrapper warper) => warper.Pattern;
    }

    public sealed record SpaceToBatchWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[SpaceToBatch.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern BlockShapePat() => Pattern[SpaceToBatch.BlockShape];
        public T BlockShapePat<T>()
            where T : ExprPattern => (T)BlockShapePat();
        public Expr BlockShape() => GetCast<Expr>(BlockShapePat());
        public T BlockShape<T>()
            where T : Expr => GetCast<T>(BlockShapePat());
        public ExprPattern PaddingsPat() => Pattern[SpaceToBatch.Paddings];
        public T PaddingsPat<T>()
            where T : ExprPattern => (T)PaddingsPat();
        public Expr Paddings() => GetCast<Expr>(PaddingsPat());
        public T Paddings<T>()
            where T : Expr => GetCast<T>(PaddingsPat());
        public static implicit operator CallPattern(SpaceToBatchWrapper warper) => warper.Pattern;
    }

    public sealed record SplitWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Split.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern AxisPat() => Pattern[Split.Axis];
        public T AxisPat<T>()
            where T : ExprPattern => (T)AxisPat();
        public Expr Axis() => GetCast<Expr>(AxisPat());
        public T Axis<T>()
            where T : Expr => GetCast<T>(AxisPat());
        public ExprPattern SectionsPat() => Pattern[Split.Sections];
        public T SectionsPat<T>()
            where T : ExprPattern => (T)SectionsPat();
        public Expr Sections() => GetCast<Expr>(SectionsPat());
        public T Sections<T>()
            where T : Expr => GetCast<T>(SectionsPat());
        public static implicit operator CallPattern(SplitWrapper warper) => warper.Pattern;
    }

    public sealed record SqueezeWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Squeeze.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern DimsPat() => Pattern[Squeeze.Dims];
        public T DimsPat<T>()
            where T : ExprPattern => (T)DimsPat();
        public Expr Dims() => GetCast<Expr>(DimsPat());
        public T Dims<T>()
            where T : Expr => GetCast<T>(DimsPat());
        public static implicit operator CallPattern(SqueezeWrapper warper) => warper.Pattern;
    }

    public sealed record TransposeWrapper(CallPattern Pattern) : PatternWrapper
    {
        public ExprPattern InputPat() => Pattern[Transpose.Input];
        public T InputPat<T>()
            where T : ExprPattern => (T)InputPat();
        public Expr Input() => GetCast<Expr>(InputPat());
        public T Input<T>()
            where T : Expr => GetCast<T>(InputPat());
        public ExprPattern PermPat() => Pattern[Transpose.Perm];
        public T PermPat<T>()
            where T : ExprPattern => (T)PermPat();
        public Expr Perm() => GetCast<Expr>(PermPat());
        public T Perm<T>()
            where T : Expr => GetCast<T>(PermPat());
        public static implicit operator CallPattern(TransposeWrapper warper) => warper.Pattern;
    }
}