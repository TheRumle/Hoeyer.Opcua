using Hoeyer.OpcUa.Core;

namespace Playground.Models;

[OpcUaEntity]
public sealed record MyLittleRobot
{
    public required string Name { get; set; }
    public required Position TargetPosition { get; set; }
    public required float Speed { get; set; }
    public string FirmwareVersion { get; set; }
}