using Hoeyer.OpcUa.Entity;

namespace Hoeyer.OpcUa.Server.Test.TestData.Entities;

[OpcUaEntity]
public class Gantry
{
    public double Speed { get; set; }
    public bool IsMoving { get; set; }
}

