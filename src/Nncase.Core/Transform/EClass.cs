﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nncase.Transform;

/// <summary>
/// EClass.
/// </summary>
public sealed class EClass
{
    private readonly List<ENode> _nodes = new();
    private List<ENode>? _used = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EClass"/> class.
    /// </summary>
    /// <param name="id">Id.</param>
    public EClass(int id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets id.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets or sets parent.
    /// </summary>
    public EClass? Parent { get; set; }

    /// <summary>
    /// Gets the Used mean which Enode use this EClass. eg. z = x + y. the EClass's Used will add {(z, z's eclass id)}.
    /// <remark> It's Not mean this EClass's Nodes </remark>
    /// </summary>
    public IReadOnlyList<ENode> Used => _used ?? throw new InvalidOperationException("This class has been merged.");

    /// <summary>
    /// Gets nodes.
    /// </summary>
    public IReadOnlyList<ENode> Nodes => _nodes;

    /// <summary>
    /// Find root eclass.
    /// </summary>
    /// <returns>Root eclass.</returns>
    public EClass Find()
    {
        if (Parent is null)
        {
            return this;
        }

        Parent = Parent.Find();
        return Parent;
    }

    /// <summary>
    /// Add enode.
    /// </summary>
    /// <param name="enode">ENode.</param>
    public void AddNode(ENode enode)
    {
        _nodes.Add(enode);
    }

    /// <summary>
    /// Add enode.
    /// </summary>
    /// <param name="enodes">ENodes.</param>
    public void AddNodes(IEnumerable<ENode> enodes)
    {
        _nodes.AddRange(enodes);
    }

    /// <summary>
    /// Remove enode.
    /// </summary>
    /// <param name="enode">ENode.</param>
    public void RemoveNode(ENode enode)
    {
        _nodes.Remove(enode);
    }

    /// <summary>
    /// Replace enode.
    /// </summary>
    /// <param name="oldNode">Old enode.</param>
    /// <param name="newNode">New enode.</param>
    public void ReplaceNode(ENode oldNode, ENode newNode)
    {
        var index = _nodes.IndexOf(oldNode);
        if (index != -1)
        {
            _nodes[index] = newNode;
        }
        else
        {
            // Original class may have been killed.
            _nodes.Add(newNode);
        }
    }

    /// <summary>
    /// Add used enode.
    /// </summary>
    /// <param name="enode">ENode.</param>
    public void AddUsed(ENode enode)
    {
        if (_used == null)
        {
            throw new InvalidOperationException("This class has been merged.");
        }

        _used.Add(enode);
    }

    /// <summary>
    /// Kill this class.
    /// </summary>
    public void Kill()
    {
        _nodes.Clear();
        _used = null;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Id} -> {Parent?.Id}";
}
