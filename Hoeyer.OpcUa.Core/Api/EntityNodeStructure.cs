using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public sealed class ValueProperty(PropertyState state)
{
    public string Name { get; } = state.BrowseName.Name;
    public NodeId NodeId { get; } = state.NodeId;
    public object Value { get; set; } = state.WrappedValue.Value;
    public object Handle { get; } = state.Handle;
}

public readonly record struct EntityNodeStructure
{
    public readonly string EntityName;
    public readonly IReadOnlyDictionary<string, NodeId> Methods;
    public readonly NodeId NodeId;
    public readonly IReadOnlyDictionary<string, ValueProperty> Properties;

    public EntityNodeStructure(IEntityNode node)
    {
        NodeId = node.BaseObject.NodeId;
        EntityName = node.BaseObject.BrowseName.Name;
        Properties =
            node.PropertyByBrowseName.ToDictionary(k => k.Value.BrowseName.Name, k => new ValueProperty(k.Value));
        Methods = node.MethodsByName.ToDictionary(e => e.Key, e => e.Value.NodeId);
    }

    public IEnumerable<ValueProperty> PropertyStates => Properties.Values;
}