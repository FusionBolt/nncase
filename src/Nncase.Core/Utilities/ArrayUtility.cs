﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nncase.Utilities;

/// <summary>
/// Array utility.
/// </summary>
public static class ArrayUtility
{
    /// <summary>
    /// Convert jagged array to 2D array.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">Jagged array.</param>
    /// <returns>2D array.</returns>
    /// <exception cref="InvalidOperationException">The given jagged array is not rectangular.</exception>
    public static unsafe T[,] To2D<T>(this T[][] source)
        where T : unmanaged
    {
        var innerLength = source[0].Length;
        var dataOut = new T[source.Length, innerLength];

        for (var i = 0; i < source.Length; i++)
        {
            if (source[i].Length != innerLength)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }

            Buffer.BlockCopy(source[i], 0, dataOut, i * innerLength * sizeof(T), innerLength * sizeof(T));
        }

        return dataOut;
    }
}
