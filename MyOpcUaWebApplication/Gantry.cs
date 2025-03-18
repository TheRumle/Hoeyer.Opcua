using Hoeyer.OpcUa.Core;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public class Gantry
{
    public IList<int> Speeds { get; set; }
    
}   