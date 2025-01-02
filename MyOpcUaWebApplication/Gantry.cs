using Hoeyer.OpcUa;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public class Gantry
{
    public int Speed { get; private set; }
    public double Speed2 { get; private set; }
    public ushort Speed3 { get; private set; }
    public string HELLO { get; private set; }
    
    public List<int> MyList { get; private set; } 
}
