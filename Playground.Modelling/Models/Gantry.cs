using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.EntityModelling.Models;

[OpcUaEntity]
public sealed class Gantry
{
    public required Position Position { get; set; }
    public required int IntValue { get; set; }
    public required Guid HeldContainer { get; set; }
    public required bool Occupied { get; set; }
    public required string StringValue { get; set; }
    public required List<string> ListValue { get; set; }
}