using Hoeyer.Machines.OpcUa.Generated.Configuration;

namespace MyOpcUaWebApplication.Sample;

[OpcNodeConfiguration]
public record Gantry(string Name, int Id, int Speed);