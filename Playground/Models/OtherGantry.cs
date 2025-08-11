using Hoeyer.OpcUa.Core;

namespace Playground.Models;

[OpcUaAgent]
public sealed class OtherGantry
{
    public int OtherGantryIntValue { get; set; }
    public string OtherGantryStringValue { get; set; }
    public List<Guid> OtherGantryAListOfSomeSort { get; set; }
}