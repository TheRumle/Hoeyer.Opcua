using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNodeHandle
{
    public BaseInstanceState Value { get; }
}