using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.EndToEndTest.TestApplication;

[OpcUaEntity]
public sealed class Gantry
{
    public int IntValue { get; set; }
    public string StringValue { get; set; }
    public List<string> AList { get; set; } 
}