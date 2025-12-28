using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public abstract class LimitAlarmAttribute<T>(
    T? lower,
    T? upper,
    string browseName,
    AlarmSeverity severity)
    : OpcAlarmAttribute(AlarmType.Limit, browseName, severity)
    where T : IComparable<T>
{
    public T? Lower { get; } = lower;
    public T? Upper { get; } = upper;
}