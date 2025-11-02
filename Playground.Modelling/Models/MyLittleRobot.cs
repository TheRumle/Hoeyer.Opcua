using Hoeyer.OpcUa.Core;

namespace Playground.Modelling.Models;

[OpcUaEntity]
public sealed record MyLittleRobot
{
    public required string Name { get; set; }
    public required Position TargetPosition { get; set; }
    public required float Speed { get; set; }
    public required string FirmwareVersion { get; set; }
}