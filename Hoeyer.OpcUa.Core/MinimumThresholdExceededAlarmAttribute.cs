using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public sealed class MinimumThresholdExceededAlarmAttribute<T>(
    T minimumThreshold,
    string browseName,
    AlarmSeverity severity)
    : LimitAlarmAttribute<T>(minimumThreshold, default, browseName, severity)
    where T : IComparable<T>
{
}