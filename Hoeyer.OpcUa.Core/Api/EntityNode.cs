using System.Collections.Frozen;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public sealed record EntityNode : IEntityNode
{
    public EntityNode(BaseObjectState baseObject,
        ISet<PropertyState> propertyStates,
        ISet<MethodState> methods,
        AlarmCollection alarms)
    {
        BaseObject = baseObject;
        MethodsByName = methods.ToFrozenDictionary(e => e.BrowseName.Name, e => e);
        PropertyByBrowseName = propertyStates.ToFrozenDictionary(e => e.BrowseName.Name);
        PropertyAlarmsByName = alarms.PropertyAlarms.ToFrozenDictionary(e => e.BrowseName.Name, e => e);
    }

    public IReadOnlyDictionary<string, LimitAlarmState> PropertyAlarmsByName { get; set; }
    public BaseObjectState BaseObject { get; }
    public IEnumerable<PropertyState> PropertyStates => PropertyByBrowseName.Values;
    public IEnumerable<MethodState> Methods => MethodsByName.Values;
    public IEnumerable<LimitAlarmState> PropertyAlarms => PropertyAlarmsByName.Values;
    public IReadOnlyDictionary<string, PropertyState> PropertyByBrowseName { get; }
    public IReadOnlyDictionary<string, MethodState> MethodsByName { get; }
}