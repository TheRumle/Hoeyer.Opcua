using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.PropertyAccess;

[OpcUaEntity]
public class ProtecectedStaticSecondClass
{
    protected static string ProtecectedStaticSecond { get; private set; }
}