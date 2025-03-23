using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Management;

internal sealed record ManagedEntityNode(
    BaseObjectState BaseObject,
    Dictionary<NodeId, PropertyState> PropertyStates,
    string Namespace,
    ushort EntityNameSpaceIndex) : IEntityNode
{
    public ManagedEntityNode(IEntityNode node, string entityNamespace, ushort entityNamespaceIndex)
        : this(node.BaseObject, node.PropertyStates, entityNamespace, entityNamespaceIndex)
    {
    }

    public string Namespace { get; } = Namespace;
    public ushort EntityNameSpaceIndex { get; } = EntityNameSpaceIndex;
    public BaseObjectState BaseObject { get; } = BaseObject;
    public Dictionary<NodeId, PropertyState> PropertyStates { get; } = PropertyStates;

    public Dictionary<string, PropertyState> PropertyByBrowseName =>
        PropertyStates.ToDictionary(e => e.Value.BrowseName.Name, e => e.Value);

    public string GetNameOfManaged(NodeId nodeId)
    {
        if (BaseObject.NodeId.Equals(nodeId)) return BaseObject.DisplayName.ToString();
        if (PropertyStates.TryGetValue(nodeId, out var propertyState)) return propertyState.DisplayName.ToString();
        return "???";
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return JsonSerializer.Serialize(new
        {
            Name = BaseObject.DisplayName.ToString(),
            Id = BaseObject.NodeId.ToString(),
            Namespace = EntityNameSpaceIndex.ToString(),
            State = PropertyStates.Values.Select(e => new
            {
                Name = e.DisplayName.ToString(),
                Value = e.Value.ToString(),
                Id = e.NodeId.ToString()
            })
        }, new JsonSerializerOptions { WriteIndented = true });
    }
}