using System.Collections.Frozen;
using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public sealed record Agent : IAgent
{
    public Agent(BaseObjectState baseObject,
        ISet<PropertyState> propertyStates,
        ISet<MethodState> methods)
    {
        BaseObject = baseObject;
        MethodsByName = methods.ToFrozenDictionary(e => e.BrowseName.Name, e => e);
        PropertyByBrowseName = propertyStates.ToFrozenDictionary(e => e.BrowseName.Name);
    }

    public BaseObjectState BaseObject { get; }
    public IEnumerable<PropertyState> PropertyStates => PropertyByBrowseName.Values;
    public IEnumerable<MethodState> Methods => MethodsByName.Values;
    public IReadOnlyDictionary<string, PropertyState> PropertyByBrowseName { get; }
    public IReadOnlyDictionary<string, MethodState> MethodsByName { get; }
}