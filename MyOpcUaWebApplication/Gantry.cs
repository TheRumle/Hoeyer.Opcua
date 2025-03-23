using Hoeyer.OpcUa.Core;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public sealed class Gantry
{
    public List<int> Speeds { get; set; }
    public string message { get; set; }
    public HashSet<string> messages { get; set; }
}