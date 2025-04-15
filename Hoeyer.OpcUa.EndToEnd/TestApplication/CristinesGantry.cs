using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.EndToEndTest.TestApplication;

[OpcUaEntity]
public sealed class CristinesGantry
{
    public int IntValue { get; set; }
    public string StringValue { get; set; }
    public List<string> AList { get; set; } 
}