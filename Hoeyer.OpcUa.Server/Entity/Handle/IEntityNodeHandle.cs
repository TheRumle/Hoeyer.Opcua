using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Handle;

public interface IEntityNodeHandle
{
    internal BaseInstanceState Value { get; }
    internal NodeId DataTypeDefinitionId { get; }
}