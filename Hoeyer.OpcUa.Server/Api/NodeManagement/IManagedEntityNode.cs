﻿using System;
using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

/// <summary>
/// A wrapper around an <see cref="IEntityNode"/> ensuring exposing state-change methods that can be used to securely operate on the node. For changing the state of the node see <see cref="ChangeState"/>. To compute expressions over the node, use <see cref="Select{TOut}"/>.
/// </summary>
public interface IManagedEntityNode
{
    /// <summary>
    /// Locks the node and changes its state
    /// </summary>
    public void ChangeState(Action<IEntityNode> stateChanges);

    /// <summary>
    /// Lock the node and compute an expression over it
    /// </summary>
    /// <param name="computation">The computation to perform over the node</param>
    /// <typeparam name="TOut">The type of the selected value</typeparam>
    /// <returns>The selected value</returns>
    public TOut Select<TOut>(Func<IEntityNode, TOut> computation);

    /// <summary>
    /// Lock the node and examine its state and execute side-effects based on it
    /// </summary>
    /// <param name="effect">The computation to perform over the node</param>
    public void Examine(Action<IEntityNode> effect);
    
    string Namespace { get; }
    ushort EntityNameSpaceIndex { get; }

    string EntityName { get; }
}

/// <inheritdoc />
public interface IManagedEntityNode<T> : IManagedEntityNode;