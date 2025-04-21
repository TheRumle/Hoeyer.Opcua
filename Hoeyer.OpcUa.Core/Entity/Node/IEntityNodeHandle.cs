using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNodeHandle
{
    public BaseInstanceState Value { get; }
}


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
}