using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public sealed record EntityNode : IEntityNode
{
    public EntityNode(BaseObjectState baseObject,
        ISet<PropertyState> propertyStates,
        ISet<MethodState> methods)
    {
        BaseObject = baseObject;
        MethodsByName = methods.ToDictionary(e => e.BrowseName.Name, e => e);
        PropertyByBrowseName = propertyStates.ToDictionary(e => e.BrowseName.Name);
    }

    public BaseObjectState BaseObject { get; }
    public IEnumerable<PropertyState> PropertyStates => PropertyByBrowseName.Values;
    public IEnumerable<MethodState> Methods => MethodsByName.Values;
    public Dictionary<string, PropertyState> PropertyByBrowseName { get; }
    public Dictionary<string, MethodState> MethodsByName { get; }
}