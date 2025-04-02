using Hoeyer.OpcUa.Core;

namespace MyOpcUaWebApplication;

public enum Position
{
    OverThere,
    OverHere,
    OnTheMoon
}

[OpcUaEntity]
public sealed class Gantry
{
    public Position Position { get; set; }
    public bool Moving { get; set; }
    public List<int> Speeds { get; set; }
    public string message { get; set; }
    public List<string> messages { get; set; }

}