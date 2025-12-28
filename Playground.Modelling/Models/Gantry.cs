using Hoeyer.OpcUa.Core;

namespace Playground.Modelling.Models;

[OpcUaEntity]
public sealed class Gantry
{
    [BrowseName("GantryPosition")] public required Position Position { get; set; }

    [LegalRangeAlarm(0, 10, "IntValueAlarm", AlarmSeverity.Critical)]
    public required int IntValue { get; set; }

    public required Guid HeldContainer { get; set; }
    public required bool Occupied { get; set; }
    public required string StringValue { get; set; }
    public required List<string> ListValue { get; set; }
}