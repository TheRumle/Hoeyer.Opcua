namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class ProtecectedSecondClass
{
    protected string ProtecectedSecond { get; private set; }
}