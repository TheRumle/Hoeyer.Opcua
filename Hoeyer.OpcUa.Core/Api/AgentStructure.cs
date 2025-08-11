using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public readonly record struct AgentStructure
{
    public readonly string EntityName;
    public readonly IReadOnlyDictionary<string, NodeId> Methods;
    public readonly NodeId NodeId;
    public readonly IReadOnlyDictionary<string, ValueProperty> Properties;

    public AgentStructure(IAgent node)
    {
        NodeId = node.BaseObject.NodeId;
        EntityName = node.BaseObject.BrowseName.Name;
        Properties =
            node.PropertyByBrowseName.ToDictionary(k => k.Value.BrowseName.Name, k => new ValueProperty(k.Value));
        Methods = node.MethodsByName.ToDictionary(e => e.Key, e => e.Value.NodeId);
    }

    public IEnumerable<ValueProperty> PropertyStates => Properties.Values;
}