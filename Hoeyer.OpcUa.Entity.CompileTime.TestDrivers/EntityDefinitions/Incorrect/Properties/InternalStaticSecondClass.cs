using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class InternalStaticSecondClass
{
    internal string InternalStaticSecond { get; private set; }
}