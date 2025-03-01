using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class ProtecectedStaticFirstClass
{
    protected static string ProtecectedStaticFirst { get; }
}