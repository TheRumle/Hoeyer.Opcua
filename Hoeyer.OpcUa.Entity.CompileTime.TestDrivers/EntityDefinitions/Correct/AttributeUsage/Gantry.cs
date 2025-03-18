using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Correct.AttributeUsage;

[OpcUaEntity]
public class Gantry
{
    public IList<int> Speeds { get; set; }
    
}