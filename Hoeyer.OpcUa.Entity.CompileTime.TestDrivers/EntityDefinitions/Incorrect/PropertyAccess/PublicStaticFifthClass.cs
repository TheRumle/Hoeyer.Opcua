using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.PropertyAccess;

[OpcUaEntity]
public class PublicStaticFifthClass
{
    public static string PublicStaticFifth { get; protected internal set; }
}