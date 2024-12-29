using Hoeyer.OpcUa;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public record Gantry
{
    public int Speed { get; init; }
}

