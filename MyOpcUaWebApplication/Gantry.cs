using Hoeyer.OpcUa.Core.Entity;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public class Gantry
{
    public int Speed { get; set; }
    public IEnumerable<int> Speeds { get; set; }

}