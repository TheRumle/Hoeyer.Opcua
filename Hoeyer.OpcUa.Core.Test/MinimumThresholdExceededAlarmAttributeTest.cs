using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Test;

public class MinimumThresholdExceededAlarmAttributeTest
{
    private const AlarmSeverity SEVERITY = AlarmSeverity.Critical;
    private const string BROWSENAME = "THRESHOLD_NAME";

    [Test]
    [Arguments(9, 9, 10, LimitAlarmStates.LowLow)]
    [Arguments(10, 9, 10, LimitAlarmStates.Low)]
    [Arguments(11, 10, 10, LimitAlarmStates.Inactive)]
    public async Task GivenValue_ReportsAlarmStateCorrectly(
        double value, double lowLow, double low, LimitAlarmStates expected)
    {
        var attribute = GetMaximumThresholdAttribute(lowLow, low);
        var alarmState = attribute.ToMap().Invoke(value);
        await Assert.That(alarmState).IsEqualTo(expected);
    }

    private static MinimumThresholdExceededAlarmAttribute GetMaximumThresholdAttribute(double lowLow, double low) =>
        new(lowLow, low, BROWSENAME, SEVERITY);
}