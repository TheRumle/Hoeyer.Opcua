using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Field)]
public sealed class OpcUaAlarmTypeAttribute(AlarmValue alarmValue) : Attribute
{
    public readonly AlarmValue AlarmValue = alarmValue;
}