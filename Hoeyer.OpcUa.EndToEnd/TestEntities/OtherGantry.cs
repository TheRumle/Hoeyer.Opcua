using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.EndToEndTest.TestEntities;

[OpcUaEntity]
public sealed class OtherGantry
{
    public int IntValue { get; set; }
    public string StringValue { get; set; }
    public List<string> AList { get; set; } 
}