using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Test;

public class MaximumThresholdExceededAlarmAttributeTest
{
    private const AlarmSeverity SEVERITY = AlarmSeverity.Critical;
    private const string BROWSENAME = "THRESHOLD_NAME";

    [Test]
    [Arguments(11, 11, 11, LimitAlarmStates.HighHigh)]
    [Arguments(10, 10, 11, LimitAlarmStates.High)]
    [Arguments(9, 10, 11, LimitAlarmStates.Inactive)]
    public async Task GivenValue_ReportsAlarmStateCorrectly(
        double value, double high, double highHigh, LimitAlarmStates expected)
    {
        var attribute = GetMaximumThresholdAttribute(high, highHigh);
        var alarmState = attribute.ToMap().Invoke(value);
        await Assert.That(alarmState).IsEqualTo(expected);
    }

    private static MaximumThresholdExceededAlarmAttribute GetMaximumThresholdAttribute(double high, double highHigh) =>
        new(high, highHigh, BROWSENAME, SEVERITY);
}