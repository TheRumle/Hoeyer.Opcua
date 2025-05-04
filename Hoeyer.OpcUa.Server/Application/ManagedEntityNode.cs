using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed record ManagedEntityNode<T>(
    BaseObjectState BaseObject,
    ISet<PropertyState> PropertyStates,
    string Namespace,
    ushort EntityNameSpaceIndex) : IManagedEntityNode
{
    public ManagedEntityNode(IEntityNode node, string entityNamespace, ushort entityNamespaceIndex)
        : this(node.BaseObject, node.PropertyStates, entityNamespace, entityNamespaceIndex)
    {
    }

    public string Namespace { get; } = Namespace;
    public ushort EntityNameSpaceIndex { get; } = EntityNameSpaceIndex;
    public BaseObjectState BaseObject { get; } = BaseObject;
    public ISet<PropertyState> PropertyStates { get; } = PropertyStates;

    public Dictionary<string, PropertyState> PropertyByBrowseName => PropertyStates.ToDictionary(e => e.BrowseName.Name);
    
    /// <inheritdoc />
    public override string ToString()
    {
        return JsonSerializer.Serialize(new
        {
            Name = BaseObject.DisplayName.ToString(),
            Id = BaseObject.NodeId.ToString(),
            Namespace = EntityNameSpaceIndex.ToString(),
            State = PropertyStates.Select(e => new
            {
                Name = e.DisplayName.ToString(),
                Value = e.Value.ToString(),
                Id = e.NodeId.ToString()
            })
        }, new JsonSerializerOptions { WriteIndented = true });
    }
}