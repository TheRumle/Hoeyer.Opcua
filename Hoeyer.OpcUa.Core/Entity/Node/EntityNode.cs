using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNode
{
    public BaseObjectState BaseObject { get; }
    public Dictionary<NodeId, PropertyState> PropertyStates { get; }
}

public record EntityNode(BaseObjectState BaseObject, Dictionary<NodeId, PropertyState> PropertyStates)
    : IEntityNode
{
    public EntityNode(BaseObjectState entity, IEnumerable<PropertyState> propertyStates)
        : this(entity, propertyStates.ToDictionary(e => e.NodeId, e => e))
    {
    }

    public IEnumerable<BaseVariableState> AllProperties => PropertyStates.Values;


    public BaseObjectState BaseObject { get; } = BaseObject;

    public Dictionary<NodeId, PropertyState> PropertyStates { get; } = PropertyStates;
}