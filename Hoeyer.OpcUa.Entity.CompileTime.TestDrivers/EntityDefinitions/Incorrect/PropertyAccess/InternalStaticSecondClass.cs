using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.PropertyAccess;

[OpcUaEntity]
public class InternalStaticSecondClass
{
    internal string InternalStaticSecond { get; private set; }
}