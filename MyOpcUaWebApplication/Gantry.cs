using Hoeyer.OpcUa.Core;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public sealed class Gantry
{
    public Position Position { get; set; }
    public bool Moving { get; set; }
    public List<int> Speeds { get; set; }
    public string message { get; set; }
    public List<string> messages { get; set; }
    
    public event Action<int, int, int> MethodWithNoOutput;

    
    public List<string> Names { get; set; } = ["rasmus", "christmas"];
    
}