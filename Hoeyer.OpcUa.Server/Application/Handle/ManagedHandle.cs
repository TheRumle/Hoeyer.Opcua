using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Handle;

internal abstract record ManagedHandle<T> : IEntityNodeHandle
    where T : BaseInstanceState
{
    protected ManagedHandle(T payload, NodeId dataTypeDefinitionId)
    {
        Value = payload;
        Payload = payload;
        DataTypeDefinitionId = dataTypeDefinitionId;
    }

    public T Payload { get; }

    /// <inheritdoc />
    public NodeId DataTypeDefinitionId { get; }


    /// <inheritdoc />
    public BaseInstanceState Value { get; }
}