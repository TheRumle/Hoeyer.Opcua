using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public class LegalRangeAlarmAttribute(
    double minimumWarningThreshold,
    double minimumDangerThreshold,
    double maximumWarningThreshold,
    double maximumDangerThreshold,
    string browseName,
    AlarmSeverity severity)
    : LimitAlarmAttribute<double>(
        minimumDangerThreshold,
        minimumWarningThreshold,
        maximumWarningThreshold,
        maximumDangerThreshold,
        browseName,
        severity)
{
    public LegalRangeAlarmAttribute(
        double minimumDangerThreshold,
        double maximumDangerThreshold,
        string browseName,
        AlarmSeverity severity) : this(
        minimumDangerThreshold,
        minimumDangerThreshold,
        maximumDangerThreshold,
        maximumDangerThreshold,
        browseName, severity)
    {
    }
}