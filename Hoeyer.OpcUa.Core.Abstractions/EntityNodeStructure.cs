using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions;

public readonly record struct EntityNodeStructure
{
    public readonly string EntityName;
    public readonly IReadOnlyDictionary<string, NodeId> Methods;
    public readonly NodeId NodeId;
    public readonly IReadOnlyDictionary<string, PropertyState> Properties;

    public EntityNodeStructure(IEntityNode node)
    {
        NodeId = node.BaseObject.NodeId;
        EntityName = node.BaseObject.BrowseName.Name;
        Properties =
            node.PropertyByBrowseName.ToDictionary(k => k.Value.BrowseName.Name, k => k.Value);
        Methods = node.MethodsByName.ToDictionary(e => e.Key, e => e.Value.NodeId);
    }
}