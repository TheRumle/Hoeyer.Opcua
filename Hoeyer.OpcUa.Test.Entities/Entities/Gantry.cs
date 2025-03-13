using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Test.Entities;

[OpcUaEntity]
public class Gantry
{
    public int Speed { get; set; }
    public bool IsMoving { get; set; }
}