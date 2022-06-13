﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nncase.IR;
using Nncase.Runtime.StackVM;

namespace Nncase.CodeGen.StackVM;

/// <summary>
/// StackVM module builder.
/// </summary>
public class StackVMModuleBuilder : IModuleBuilder
{
    private readonly MemoryStream _rdataContent = new MemoryStream();
    private readonly BinaryWriter _rdataWriter;

    /// <summary>
    /// Initializes a new instance of the <see cref="StackVMModuleBuilder"/> class.
    /// </summary>
    public StackVMModuleBuilder()
    {
        _rdataWriter = new BinaryWriter(_rdataContent, Encoding.UTF8, leaveOpen: true);
    }

    /// <inheritdoc/>
    public string ModuleKind => StackVMRTModule.Kind;

    /// <inheritdoc/>
    public ILinkableModule Build(IReadOnlyList<Callable> functions)
    {
        var linkableFunctions = Compile(functions.Cast<Function>());
        _rdataWriter.Flush();
        return new LinkableModule(_rdataContent.ToArray(), linkableFunctions);
    }

    private LinkableFunction[] Compile(IEnumerable<Function> functions)
    {
        return functions.Select((f, i) => new StackVMFunctionBuilder((uint)i, _rdataWriter).Build(f)).ToArray();
    }
}