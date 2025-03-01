using Hoeyer.OpcUa.Entity;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public class Gantry
{
    public int Speed { get; set; }
    public bool IsMoving { get; set; }
}