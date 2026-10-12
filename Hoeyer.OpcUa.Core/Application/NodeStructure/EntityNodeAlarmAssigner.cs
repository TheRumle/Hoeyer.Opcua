using System.Collections.Frozen;
using Hoeyer.OpcUa.Core.Abstractions.Alarm;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class EntityNodeAlarmAssigner<T>(IEntityTypeModel<T> model) : IEntityNodeAlarmAssigner<T>
{
    public AlarmCollection AssignAlarms(IEnumerable<PropertyState> properties)
    {
        var propertyStates = properties.ToDictionary(property => property.BrowseName.Name!, property => property);
        var alarms = GetAlarms(propertyStates).ToFrozenDictionary(e => e.property, e => e.alarm);
        return new AlarmCollection(alarms);
    }

    private IEnumerable<(PropertyState property, LimitAlarmState alarm)> GetAlarms(
        Dictionary<string, PropertyState> propertyStates)
    {
        foreach (var (propertyBrowseName, alarm) in model.PropertyAlarms)
        {
            var property = propertyStates[propertyBrowseName];
            foreach (var alarmAttribute in alarm)
            {
                switch (alarmAttribute)
                {
                    case LegalRangeAlarmAttribute limitAlarm:
                        yield return (property, AssignLimitAlarm(property, limitAlarm));
                        break;
                }
            }
        }
    }

    private static ExclusiveLimitAlarmState AssignLimitAlarm(
        NodeState property,
        LimitAlarmAttribute<double> alarm)
    {
        var alarmNode = new ExclusiveLimitAlarmState(property);
        alarmNode.LowLowLimit = CreateLimitNode<double, VariantBuilder>(alarm.LowLow, alarmNode);
        alarmNode.LowLimit = CreateLimitNode<double, VariantBuilder>(alarm.Low, alarmNode);
        alarmNode.HighHighLimit = CreateLimitNode<double, VariantBuilder>(alarm.HighHigh, alarmNode);
        alarmNode.HighLimit = CreateLimitNode<double, VariantBuilder>(alarm.High, alarmNode);
        alarmNode.Severity = CreateLimitNode<ushort, VariantBuilder>(TranslateSeverity(alarm.Severity), alarmNode);
        alarmNode.BrowseName = new QualifiedName(alarm.BrowseName, property.BrowseName.NamespaceIndex);
        alarmNode.DisplayName = new LocalizedText(alarmNode.BrowseName.Name);
        alarmNode.TypeDefinitionId = ObjectTypeIds.LimitAlarmType;
        alarmNode.InputNode = PropertyState<NodeId>.With<VariantBuilder>(alarmNode);
        alarmNode.InputNode.Value = property.NodeId;

        return alarmNode;
    }

    private static PropertyState<T> CreateLimitNode<T, TVariantBuilder>
    (
        T value,
        ExclusiveLimitAlarmState alarmNode
    ) where TVariantBuilder : struct, IVariantBuilder<T>
    {
        var state = PropertyState<T>.With<TVariantBuilder>(alarmNode);
        state.Value = value;
        return state;
    }

    private static ushort TranslateSeverity(AlarmSeverity alarmSeverity) =>
        alarmSeverity switch
        {
            AlarmSeverity.Low => 100, // Low Severity (1–199)
            AlarmSeverity.Medium => 300, // Medium Severity (200–399)
            AlarmSeverity.High => 500, // High Severity (400–599)
            AlarmSeverity.Critical => 800, // Critical Severity (600–999)
            var _ => 0
        };
}