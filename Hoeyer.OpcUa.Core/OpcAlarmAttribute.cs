using System;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public abstract class OpcAlarmAttribute(AlarmType alarmType, string browseName, AlarmSeverity severity)
    : Attribute, IOpcAlarm
{
    public AlarmType AlarmType => alarmType;
    public string BrowseName => browseName;
    public AlarmSeverity Severity => severity;
}