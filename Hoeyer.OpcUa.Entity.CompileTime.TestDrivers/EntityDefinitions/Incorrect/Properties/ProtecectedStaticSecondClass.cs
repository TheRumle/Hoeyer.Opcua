using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class ProtecectedStaticSecondClass
{
    protected static string ProtecectedStaticSecond { get; private set; }
}