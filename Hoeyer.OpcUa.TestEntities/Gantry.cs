using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.TestEntities.Methods;

namespace Hoeyer.OpcUa.TestEntities;

[OpcUaAgent]
public sealed class Gantry
{
    public required Position Position { get; set; }
    public required int IntValue { get; set; }
    public required Guid HeldContainer { get; set; }
    public required bool Occupied { get; set; }
    public required string StringValue { get; set; }
    public required List<string> AList { get; set; }
    public required List<string> AAginList { get; set; }
}