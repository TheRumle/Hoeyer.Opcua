using System.Collections.Frozen;
using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public sealed record AlarmCollection
{
    public AlarmCollection(FrozenDictionary<PropertyState, LimitAlarmState> alarmsByProperty)
    {
        PropertyAlarms = alarmsByProperty.Values;
        AlarmsByProperty = alarmsByProperty;
    }

    public FrozenDictionary<PropertyState, LimitAlarmState> AlarmsByProperty { get; set; }

    internal IList<LimitAlarmState> PropertyAlarms { get; } = new List<LimitAlarmState>();
}