using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.TestEntities.Models;

[OpcUaEntity]
public sealed class GantryNumberTwo
{
    public Position Position { get; set; }
    public bool Moving { get; set; }
    public List<int> Speeds { get; set; }
    public string message { get; set; }
    public List<string> messages { get; set; }


    public List<string> Names { get; set; } = ["rasmus", "christmas"];
}