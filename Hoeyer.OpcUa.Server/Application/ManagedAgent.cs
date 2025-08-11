using System;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed record ManagedAgent<T> : IManagedAgent<T>
{
    private readonly object _lock = new();
    private readonly IAgent _managedNode;

    public ManagedAgent(IAgent node, string entityNamespace, ushort entityNamespaceIndex)
    {
        _managedNode = node;
        EntityNameSpaceIndex = entityNamespaceIndex;
        Namespace = entityNamespace;
        EntityName = _managedNode.BaseObject.BrowseName.Name;
    }

    /// <inheritdoc />
    public void Examine(Action<IAgent> effect)
    {
        lock (_lock)
        {
            effect(_managedNode);
        }
    }

    public string Namespace { get; }
    public ushort EntityNameSpaceIndex { get; }

    public string EntityName { get; set; }

    public void ChangeState(Action<IAgent> stateChanges)
    {
        lock (_lock)
        {
            stateChanges.Invoke(_managedNode);
        }
    }

    /// <inheritdoc />
    public TOut Select<TOut>(Func<IAgent, TOut> selection)
    {
        lock (_lock)
        {
            return selection.Invoke(_managedNode);
        }
    }
}