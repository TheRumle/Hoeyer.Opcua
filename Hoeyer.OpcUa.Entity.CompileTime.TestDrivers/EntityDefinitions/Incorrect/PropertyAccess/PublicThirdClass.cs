using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.PropertyAccess;

[OpcUaEntity]
public class PublicThirdClass
{
    public string PublicThird { get; protected set; }
}