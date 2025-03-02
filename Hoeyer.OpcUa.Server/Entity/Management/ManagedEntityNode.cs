using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Core.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Management;

internal sealed record ManagedEntityNode(
    BaseObjectState Entity,
    Dictionary<NodeId, PropertyState> PropertyStates,
    string Namespace,
    ushort EntityNameSpaceIndex) : IEntityNode
{
    public ManagedEntityNode(IEntityNode node, string entityNamespace, ushort entityNamespaceIndex)
        : this(node.Entity, node.PropertyStates, entityNamespace, entityNamespaceIndex)
    {
    }

    public string Namespace { get; } = Namespace;
    public ushort EntityNameSpaceIndex { get; } = EntityNameSpaceIndex;
    public BaseObjectState Entity { get; } = Entity;
    public Dictionary<NodeId, PropertyState> PropertyStates { get; } = PropertyStates;

    public string GetNameOfManaged(NodeId nodeId)
    {
        if (Entity.NodeId.Equals(nodeId)) return Entity.DisplayName.ToString();
        if (PropertyStates.TryGetValue(nodeId, out var propertyState)) return propertyState.DisplayName.ToString();
        return "???";
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $$"""
                 {{nameof(ManagedEntityNode)}} {
                   Name: {{Entity.DisplayName}},
                   Id: {{Entity.NodeId}},
                   Namespace: {{EntityNameSpaceIndex}},
                   State: [
                       {{PropertyStates.Values.Select(e => $"{{{e.DisplayName}: {e.Value}, Id: \"{e.NodeId}\"}}").SeparateBy(",\n")}}
                   ]
                 }
                 """;
    }
}