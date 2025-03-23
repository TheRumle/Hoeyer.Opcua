using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Test.Entities;

[OpcUaEntity]
public class Gantry
{
    public int Speed { get; set; }
    public bool IsMoving { get; set; }
}