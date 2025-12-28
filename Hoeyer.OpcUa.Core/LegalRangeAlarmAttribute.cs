using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public sealed class LegalRangeAlarmAttribute(
    double minimumThreshold,
    double maximumThreshold,
    string browseName,
    AlarmSeverity severity)
    : LimitAlarmAttribute<double>(minimumThreshold, maximumThreshold, browseName, severity)
{
}