using Hoeyer.OpcUa.Core;

namespace Hoeyer.opcUa.TestEntities;

[OpcUaEntity]
public sealed class Gantry
{
    public int IntValue { get; set; }
    public string StringValue { get; set; }
    public List<string> AList { get; set; }
    public List<string> AAginList { get; set; }
}