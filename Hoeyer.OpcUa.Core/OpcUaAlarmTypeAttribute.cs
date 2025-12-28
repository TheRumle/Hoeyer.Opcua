using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Field)]
public sealed class OpcUaAlarmTypeAttribute(AlarmType alarmType) : Attribute
{
    public readonly AlarmType AlarmType = alarmType;
}