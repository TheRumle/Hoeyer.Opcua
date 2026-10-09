using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Abstractions.Alarm;

namespace Hoeyer.OpcUa.Fixtures.Server.Entities;

[OpcUaEntity]
public class AlarmTestEntity
{
    [MaximumThresholdExceededAlarm(0, 1, "MyAlarm", AlarmSeverity.Critical)]
    public int Value { get; set; }
}