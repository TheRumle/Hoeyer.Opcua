using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public sealed record EntityNode(BaseObjectState BaseObject, Dictionary<NodeId, PropertyState> PropertyStates)
    : IEntityNode
{
    public readonly IEnumerable<BaseVariableState> AllProperties = PropertyStates.Values;

    public EntityNode(BaseObjectState entity, IEnumerable<PropertyState> propertyStates)
        : this(entity, propertyStates.ToDictionary(e => e.NodeId, e => e))
    {
    }

    /// <inheritdoc />
    public BaseObjectState BaseObject { get; } = BaseObject;

    public Dictionary<NodeId, PropertyState> PropertyStates { get; } = PropertyStates;

    public Dictionary<string, PropertyState> PropertyByBrowseName { get; } =
        PropertyStates.ToDictionary(e => e.Value.BrowseName.Name, e => e.Value);
}