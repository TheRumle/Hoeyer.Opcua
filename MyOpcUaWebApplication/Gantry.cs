using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Entity;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public class Gantry
{
    public HashSet<int> Speed { get; set; }

}
