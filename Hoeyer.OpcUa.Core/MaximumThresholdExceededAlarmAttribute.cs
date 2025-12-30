using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public sealed class MaximumThresholdExceededAlarmAttribute(
    double highWarningThreshold,
    double highDangerThreshold,
    string browseName,
    AlarmSeverity severity)
    : LegalRangeAlarmAttribute(double.MinValue, double.MinValue, highWarningThreshold, highDangerThreshold, browseName,
        severity);