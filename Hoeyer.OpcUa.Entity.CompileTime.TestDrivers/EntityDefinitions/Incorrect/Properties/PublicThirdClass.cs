using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class PublicThirdClass
{
    public string PublicThird { get; protected set; }
}