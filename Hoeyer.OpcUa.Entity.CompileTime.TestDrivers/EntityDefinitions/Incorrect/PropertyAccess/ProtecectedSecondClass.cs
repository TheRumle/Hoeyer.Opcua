using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.PropertyAccess;

[OpcUaEntity]
public class ProtecectedSecondClass
{
    protected string ProtecectedSecond { get; private set; }
}