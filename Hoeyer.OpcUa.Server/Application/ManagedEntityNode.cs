using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed record ManagedEntityNode<T> : IManagedEntityNode<T>
{
    public ManagedEntityNode(IEntityNode node, string entityNamespace, ushort entityNamespaceIndex)
    {
        this.BaseObject = node.BaseObject;
        this.Namespace = entityNamespace;
        this.EntityNameSpaceIndex = entityNamespaceIndex;
        this.PropertyStates = node.PropertyStates;
        this.Methods = node.Methods;
    }

    public object Lock { get; } = new();
    public string Namespace { get; }
    public ushort EntityNameSpaceIndex { get; }
    public BaseObjectState BaseObject { get; }
    public IEnumerable<PropertyState> PropertyStates { get; }
    public IEnumerable<MethodState> Methods { get; }

    public Dictionary<string, PropertyState> PropertyByBrowseName =>
        PropertyStates.ToDictionary(e => e.BrowseName.Name);

    public Dictionary<string, MethodState> MethodsByName => Methods.ToDictionary(e => e.BrowseName.Name);

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