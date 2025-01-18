namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class PublicPrivateSetter
{
    public string Fourth { get; private set; }
}