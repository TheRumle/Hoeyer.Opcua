using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Entity;

public interface IEntityNode
{
    public BaseObjectState Entity { get; }
    public FolderState Folder { get; }

    public Dictionary<NodeId, PropertyState> PropertyStates { get; }
}

public record EntityNode(FolderState Folder, BaseObjectState Entity, Dictionary<NodeId, PropertyState> PropertyStates)
    : IEntityNode
{
    public EntityNode(FolderState folder, BaseObjectState entity, IEnumerable<PropertyState> propertyStates)
    : this(folder, entity, propertyStates.ToDictionary(e=>e.NodeId, e=>e)) 
    {}
    
    
    public BaseObjectState Entity { get; } = Entity;
    public FolderState Folder { get; } = Folder;

    public Dictionary<NodeId, PropertyState> PropertyStates { get; } = PropertyStates;
    public IEnumerable<PropertyState> AllProperties => PropertyStates.Values;
}