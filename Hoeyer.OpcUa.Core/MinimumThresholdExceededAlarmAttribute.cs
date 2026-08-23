namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public sealed class MinimumThresholdExceededAlarmAttribute(
    double lowDangerThreshold,
    double lowWarningThreshold,
    string browseName,
    AlarmSeverity severity)
    : LegalRangeAlarmAttribute(lowDangerThreshold, lowWarningThreshold, double.MaxValue, double.MaxValue,
        browseName, severity)
{
}