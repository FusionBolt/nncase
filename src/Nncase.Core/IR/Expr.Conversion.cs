﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics.Tensors;

namespace Nncase.IR
{
    /// <summary>
    /// Conversion of expression.
    /// </summary>
    public abstract partial record Expr
    {
        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="byte"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(byte value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(ushort value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="uint"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(uint value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="ulong"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(ulong value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(sbyte value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="short"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(short value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="int"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(int value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="long"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(long value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="Half"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(Half value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="float"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(float value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="double"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(double value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="BFloat16"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(BFloat16 value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="bool"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Expr(bool value) => (Const)value;

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="Shape"/>.
        /// </summary>
        /// <param name="shape">Shape.</param>
        public static implicit operator Expr(Shape shape) => Const.FromShape(shape);

        /// <summary>
        /// Create <see cref="Expr"/> from an array of<see cref="int"/>.
        /// </summary>
        /// <param name="array">Array.</param>
        public static implicit operator Expr(int[] array) => Tensor.FromSpan<int>(array);

        /// <summary>
        /// Create <see cref="Expr"/> from an array of<see cref="int"/>.
        /// </summary>
        /// <param name="array">Array.</param>
        public static implicit operator Expr(long[] array) => Tensor.FromSpan<long>(array);

        /// <summary>
        /// Create <see cref="Expr"/> from an array of<see cref="float"/>.
        /// </summary>
        /// <param name="array">Array.</param>
        public static implicit operator Expr(float[] array) => Tensor.FromSpan<float>(array);

        /// <summary>
        /// Create <see cref="Expr"/> from an array of<see cref="int"/>.
        /// </summary>
        /// <param name="array">Array.</param>
        public static implicit operator Expr(Array array) => Tensor.FromArray(array);

        /// <summary>
        /// Create <see cref="Expr"/> from a span of<see cref="int"/>.
        /// </summary>
        /// <param name="span">Span.</param>
        public static implicit operator Expr(ReadOnlySpan<int> span) => Tensor.FromSpan<int>(span);

        /// <summary>
        /// Create <see cref="Expr"/> from a span of<see cref="long"/>.
        /// </summary>
        /// <param name="span">Span.</param>
        public static implicit operator Expr(ReadOnlySpan<long> span) => Tensor.FromSpan<long>(span);

        /// <summary>
        /// Create <see cref="Expr"/> from a span of<see cref="float"/>.
        /// </summary>
        /// <param name="span">Span.</param>
        public static implicit operator Expr(ReadOnlySpan<float> span) => Tensor.FromSpan<float>(span);

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="Tensor"/>.
        /// </summary>
        /// <param name="tensor">Tensor.</param>
        public static implicit operator Expr(Tensor tensor) => Const.FromTensor(tensor);

        /// <summary>
        /// Create <see cref="Expr"/> from a <see cref="QuantParam"/>.
        /// </summary>
        /// <param name="quantParam">QuantParam.</param>
        public static implicit operator Expr(QuantParam quantParam) => Tensor.FromScalar(quantParam);
    }
}
