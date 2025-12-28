using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public abstract class OpcAlarmAttribute(AlarmType alarmType, string browseName, AlarmSeverity severity) : Attribute
{
    public readonly AlarmType AlarmType = alarmType;
    public readonly string BrowseName = browseName;
    public readonly AlarmSeverity Severity = severity;
}