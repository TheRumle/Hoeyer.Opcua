using Hoeyer.OpcUa.Core;

namespace Playground.Models;

[OpcUaAgent]
public sealed record Gantry
{
    public Position Position { get; set; }
    public int Range { get; set; }

    public bool Moving { get; set; }
    public List<int> Speeds { get; set; }
    public Guid CurrentId { get; set; } = Guid.CreateVersion7();
    public string message { get; set; }
    public List<string> messages { get; set; }

    public List<string> Names { get; set; } = ["rasmus", "christmas"];
    public List<string> BB { get; set; }
}