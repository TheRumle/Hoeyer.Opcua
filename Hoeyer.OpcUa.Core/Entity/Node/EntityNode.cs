using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public sealed record EntityNode(BaseObjectState BaseObject, ISet<PropertyState> PropertyStates)
    : IEntityNode
{
    public EntityNode(BaseObjectState BaseObject, IEnumerable<PropertyState> PropertyStates) : this(BaseObject, new HashSet<PropertyState>(PropertyStates))
    {}
    
    public BaseObjectState BaseObject { get; } = BaseObject;

    public ISet<PropertyState> PropertyStates { get; } = PropertyStates;

    public Dictionary<string, PropertyState> PropertyByBrowseName => PropertyStates.ToDictionary(e => e.BrowseName.Name);

}