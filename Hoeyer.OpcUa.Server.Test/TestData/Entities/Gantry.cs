using Hoeyer.OpcUa.Entity;

namespace Hoeyer.OpcUa.Server.Test.TestData.Entities;

[OpcUaEntity]
public class Gantry
{
    public HashSet<int> Speed { get; set; } = [];
}

