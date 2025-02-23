using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Entity;

public interface IEntityNode
{
    public BaseObjectState Entity { get; }
    public Dictionary<NodeId, PropertyState> PropertyStates { get; }
}


public record EntityNode(BaseObjectState Entity, Dictionary<NodeId, PropertyState> PropertyStates)
    : IEntityNode
{
    public EntityNode(BaseObjectState entity, IEnumerable<PropertyState> propertyStates)
    : this(entity, propertyStates.ToDictionary(e=>e.NodeId, e=>e)) 
    {}
    
    
    public BaseObjectState Entity { get; } = Entity;

    public Dictionary<NodeId, PropertyState> PropertyStates { get; } = PropertyStates;
    public IEnumerable<PropertyState> AllProperties => PropertyStates.Values;
}