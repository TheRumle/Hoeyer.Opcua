using System;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed record ManagedEntityNode<T> : IManagedEntityNode<T>
{
    private readonly IEntityNode _managedNode;

    public ManagedEntityNode(IEntityNode node, string entityNamespace, ushort entityNamespaceIndex)
    {
        _managedNode = node;
        EntityNameSpaceIndex = entityNamespaceIndex;
        Namespace = entityNamespace;
        EntityName = _managedNode.BaseObject.BrowseName.Name;
    }
    private readonly object _lock = new();

    /// <inheritdoc />
    public void Examine(Action<IEntityNode> effect)
    {
        lock (_lock)
        {
            effect(_managedNode);
        }
    }

    public  string Namespace { get; }
    public ushort EntityNameSpaceIndex { get; }

    public string EntityName { get; set; }

    public void ChangeState(Action<IEntityNode> stateChanges)
    {
        lock (_lock)
        {
            stateChanges.Invoke(_managedNode);
        }
    }
    
    /// <inheritdoc />
    public TOut Select<TOut>(Func<IEntityNode, TOut> selection)
    {
        lock (_lock)
        {
            return selection.Invoke(_managedNode);
        }
    }
}