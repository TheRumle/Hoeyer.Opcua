namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class PublicInternalSetter
{
    public string Fourth { get; internal set; }
}