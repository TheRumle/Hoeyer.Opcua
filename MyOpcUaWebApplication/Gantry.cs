using Hoeyer.OpcUa.Client.Domain;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public record Gantry
{
    public int Speed { get; init; }
}

