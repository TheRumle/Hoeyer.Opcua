using System.Collections.Frozen;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions;

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
        AlarmsByProperty = alarms.AlarmsByProperty;
    }

    public string EntityName => BaseObject.BrowseName.Name;
    public BaseObjectState BaseObject { get; }
    public IReadOnlyDictionary<string, LimitAlarmState> PropertyAlarmsByName { get; set; }
    public IEnumerable<PropertyState> PropertyStates => PropertyByBrowseName.Values;
    public IEnumerable<MethodState> Methods => MethodsByName.Values;
    public IEnumerable<LimitAlarmState> PropertyAlarms => PropertyAlarmsByName.Values;
    public IReadOnlyDictionary<PropertyState, LimitAlarmState> AlarmsByProperty { get; }
    public IReadOnlyDictionary<string, PropertyState> PropertyByBrowseName { get; }
    public IReadOnlyDictionary<string, MethodState> MethodsByName { get; }
}