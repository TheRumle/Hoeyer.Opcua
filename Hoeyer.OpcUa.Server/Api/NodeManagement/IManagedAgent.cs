using System;
using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

/// <inheritdoc />
public interface IManagedAgent<T> : IManagedAgent;

/// <summary>
/// A wrapper around an <see cref="IAgent"/> ensuring exposing state-change methods that can be used to securely operate on the node. For changing the state of the node see <see cref="ChangeState"/>. To compute expressions over the node, use <see cref="Select{TOut}"/>.
/// </summary>
public interface IManagedAgent
{
    string Namespace { get; }
    ushort AgentNameSpaceIndex { get; }

    string AgentName { get; }

    /// <summary>
    /// Locks the node and changes its state
    /// </summary>
    public void ChangeState(Action<IAgent> stateChanges);

    /// <summary>
    /// Lock the node and compute an expression over it
    /// </summary>
    /// <param name="computation">The computation to perform over the node</param>
    /// <typeparam name="TOut">The type of the selected value</typeparam>
    /// <returns>The selected value</returns>
    public TOut Select<TOut>(Func<IAgent, TOut> computation);

    /// <summary>
    /// Lock the node and examine its state and execute side-effects based on it
    /// </summary>
    /// <param name="effect">The computation to perform over the node</param>
    public void Examine(Action<IAgent> effect);
}