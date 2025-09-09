using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.EntityModelling.Models;

[OpcUaEntity]
public sealed record MyLittleRobot
{
    public required string Name { get; set; }
    public required Position TargetPosition { get; set; }
    public required float Speed { get; set; }
    public string FirmwareVersion { get; set; }
}