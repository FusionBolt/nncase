﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nncase.CodeGen;

public abstract class LinkableModule : ILinkableModule
{
    private const int _textAlignment = 8;

    private readonly IReadOnlyList<ILinkableFunction> _functions;

    public LinkableModule(IReadOnlyList<ILinkableFunction> functions, SectionManager sectionManager)
    {
        _functions = functions;
        SectionManager = sectionManager;
    }

    public SectionManager SectionManager { get; }

    public ILinkedModule Link(ILinkContext linkContext)
    {
        var linkedFunctions = new List<LinkedFunction>();
        var text = new MemoryStream();
        using (var bw = new BinaryWriter(text, Encoding.UTF8, true))
        {
            foreach (var func in _functions)
            {
                FixFunctionRefs(func, linkContext);
                bw.AlignPosition(_textAlignment);
                var textBegin = bw.Position();
                bw.Write(func.Text);
                linkedFunctions.Add(new LinkedFunction(func.Id, func.SourceFunction, (uint)textBegin, (uint)func.Text.Length, func.Sections));
            }
        }

        return CreateLinkedModule(linkedFunctions, text.ToArray());
    }

    protected abstract ILinkedModule CreateLinkedModule(IReadOnlyList<LinkedFunction> linkedFunctions, byte[] text);

    private void FixFunctionRefs(ILinkableFunction func, ILinkContext linkContext)
    {
        using var writer = new BinaryWriter(new MemoryStream(func.Text));
        foreach (var funcRef in func.FunctionRefs)
        {
            var id = linkContext.GetFunctionId(funcRef.Callable);
            var value = funcRef.Component == FunctionIdComponent.ModuleId ? id.ModuleId : id.Id;
            writer.Position(funcRef.Position);
            writer.WriteByLength(value, funcRef.Length);
        }
    }
}
