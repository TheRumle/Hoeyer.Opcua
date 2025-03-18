using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.PropertyAccess;

[OpcUaEntity]
public class PublicStaticSixthClass
{
    public static string PublicStaticSixth { get; protected set; }
}