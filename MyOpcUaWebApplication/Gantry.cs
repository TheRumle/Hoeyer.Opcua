using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public class Gantry
{
    public int Speed { get; set; }
    public IEnumerable<int> Speeds { get; set; }

}