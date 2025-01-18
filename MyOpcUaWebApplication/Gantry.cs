using Hoeyer.OpcUa.Entity;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public class Gantry{
    public HashSet<int> Speed { get; set; } = [];

}
