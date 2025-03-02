using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Handle;

internal abstract record ManagedHandle<T> : IEntityNodeHandle
where T : BaseInstanceState
{
    protected ManagedHandle(T payload, NodeId dataTypeDefinitionId)
    {
        Value = payload;
        Payload = payload;
        this.DataTypeDefinitionId = dataTypeDefinitionId;
    }
    
    
    /// <inheritdoc />
    public BaseInstanceState Value { get; }
    
    public T Payload { get; }

    /// <inheritdoc />
    public NodeId DataTypeDefinitionId { get; }
}