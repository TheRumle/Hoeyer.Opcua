using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.PropertyAccess;

[OpcUaEntity]
public class PublicStaticThirdClass
{
    public static string PublicStaticThird { get; protected set; }
}