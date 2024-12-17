using Hoeyer.Machines.OpcUa.Client.Domain;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public record Gantry
{
    public Gantry()
    {
        
    }
    public int Speed { get; init; }
}