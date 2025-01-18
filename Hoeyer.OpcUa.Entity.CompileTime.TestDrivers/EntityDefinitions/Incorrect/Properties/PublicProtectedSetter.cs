namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class PublicProtectedSetter
{
    public string Fourth { get; protected set; }
}