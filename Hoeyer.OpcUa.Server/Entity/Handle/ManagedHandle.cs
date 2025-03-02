using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Handle;

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
    public BaseInstanceState Value { get; }

    /// <inheritdoc />
    public NodeId DataTypeDefinitionId { get; }
}