using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public sealed class MaximumThresholdExceededAlarmAttribute<T>(
    T maximumThreshold,
    string browseName,
    AlarmSeverity severity)
    : LimitAlarmAttribute<T>(default, maximumThreshold, browseName, severity)
    where T : IComparable<T>;