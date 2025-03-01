using Hoeyer.OpcUa.Core.Entity;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public class Gantry
{
    public int Speed { get; set; }
    public bool IsMoving { get; set; }
    public bool B { get; set; }
}