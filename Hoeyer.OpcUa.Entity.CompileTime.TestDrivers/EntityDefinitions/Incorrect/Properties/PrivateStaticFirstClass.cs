using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class PrivateStaticFirstClass
{
    private static string PrivateStaticFirst { get; set; }
}