using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public abstract record ManagedHandle<T> : IEntityNodeHandle
    where T : BaseInstanceState
{
    protected ManagedHandle(T payload, NodeId dataTypeDefinitionId)
    {
        Value = payload;
        Payload = payload;
        DataTypeDefinitionId = dataTypeDefinitionId;
    }

    public T Payload { get; }

    public NodeId DataTypeDefinitionId { get; }


    /// <inheritdoc />
    public BaseInstanceState Value { get; }

    /// <inheritdoc />
    public NodeId NodeId => Value.NodeId;
}