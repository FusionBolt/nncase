﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nncase.IR;

namespace Nncase.TIR.Builders;

/// <summary>
/// builfer the if then else block.
/// </summary>
public interface IIfThenElseBuilder : IExprBuilder<IfThenElse>
{
    /// <summary>
    /// then block.
    /// </summary>
    /// <param name="exprs"> statements. </param>
    /// <returns> IfThenElseBuilder. </returns>
    IIfThenElseBuilder Then(params object[] exprOrBuilders);

    /// <summary>
    /// else block.
    /// </summary>
    /// <param name="exprs"> statements. </param>
    /// <returns> IfThenElseBuilder. </returns>
    IIfThenElseBuilder Else(params object[] exprOrBuilders);
}

internal class IfThenElseBuilder : IIfThenElseBuilder
{
    private readonly Expr _condition;
    private readonly List<object> _then = new();
    private readonly List<object> _else = new();

    public IfThenElseBuilder(Expr condition)
    {
        _condition = condition;
    }

    public IIfThenElseBuilder Then(params object[] exprOrBuilders)
    {
        _then.AddRange(exprOrBuilders);
        return this;
    }

    public IIfThenElseBuilder Else(params object[] exprOrBuilders)
    {
        _else.AddRange(exprOrBuilders);
        return this;
    }

    public IfThenElse Build()
    {
        return new(_condition, Sequential.Flatten(_then), Sequential.Flatten(_else));
    }
}
