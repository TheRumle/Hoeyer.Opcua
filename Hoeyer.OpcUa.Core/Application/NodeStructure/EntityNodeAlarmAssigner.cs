using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class EntityNodeAlarmAssigner<T>(IEntityTypeModel<T> model) : IEntityNodeAlarmAssigner<T>
{
    public AlarmCollection AssignAlarms(IEnumerable<PropertyState> properties)
    {
        AlarmCollection collection = new();
        var propertyStates = properties.ToDictionary(e => e.BrowseName.Name, e => e);
        collection.PropertyAlarms.AddRange(GetAlarms(propertyStates));
        return collection;
    }

    private IEnumerable<LimitAlarmState> GetAlarms(Dictionary<string, PropertyState> propertyStates)
    {
        foreach (var (propertyBrowseName, alarm) in model.PropertyAlarms)
        {
            var property = propertyStates[propertyBrowseName];
            foreach (var alarmAttribute in alarm)
            {
                switch (alarmAttribute)
                {
                    case LegalRangeAlarmAttribute limitAlarm:
                        yield return AssignLimitAlarm(property, limitAlarm);
                        break;
                }
            }
        }
    }

    private static ExclusiveLimitAlarmState AssignLimitAlarm(
        NodeState parent,
        LimitAlarmAttribute<double> alarm)
    {
        var alarmNode = new ExclusiveLimitAlarmState(parent);

        alarmNode.LowLowLimit = CreateLimitNode(alarm.LowLow, alarmNode);
        alarmNode.LowLimit = CreateLimitNode(alarm.Low, alarmNode);
        alarmNode.HighHighLimit = CreateLimitNode(alarm.HighHigh, alarmNode);
        alarmNode.HighLimit = CreateLimitNode(alarm.High, alarmNode);
        alarmNode.Severity = CreateLimitNode(TranslateSeverity(alarm.Severity), alarmNode);
        alarmNode.BrowseName = new QualifiedName(alarm.BrowseName);

        return alarmNode;
    }

    private static PropertyState<TValue> CreateLimitNode<TValue>
    (
        TValue value,
        ExclusiveLimitAlarmState alarmNode
    ) => new(alarmNode) { Value = value };

    private static ushort TranslateSeverity(AlarmSeverity alarmSeverity) =>
        alarmSeverity switch
        {
            AlarmSeverity.Low => 100, // Low severity (1–199)
            AlarmSeverity.Medium => 300, // Medium severity (200–399)
            AlarmSeverity.High => 500, // High severity (400–599)
            AlarmSeverity.Critical => 800, // Critical severity (600–999)
            var _ => 0
        };
}