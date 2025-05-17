using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Core;

namespace MyOpcUaWebApplication;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    Task MyMethod(int q, int b, List<int> dict);
}

public class a(IEntityBrowser<Gantry> browser)
{
}

[OpcUaEntity]
public sealed class Gantry
{
    public Position Position { get; set; }
    public bool Moving { get; set; }
    public List<int> Speeds { get; set; }
    public string message { get; set; }
    public List<string> messages { get; set; }

    public List<string> Names { get; set; } = ["rasmus", "christmas"];
}