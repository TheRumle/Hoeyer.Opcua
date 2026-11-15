using Hoeyer.OpcUa.Core.Abstractions.Alarm;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IOpcAlarm
{
    AlarmType AlarmType { get; }
    string BrowseName { get; }
    AlarmSeverity Severity { get; }
}