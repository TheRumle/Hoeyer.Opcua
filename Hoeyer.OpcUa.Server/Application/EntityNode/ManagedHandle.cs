using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IEntityNodeHandle
{
    internal BaseInstanceState Value { get; }
    internal NodeId DataTypeDefinitionId { get; }
}

public abstract record ManagedHandle<T> : IEntityNodeHandle
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

public record PropertyHandle : ManagedHandle<PropertyState>
{
    /// <inheritdoc />
    public PropertyHandle(PropertyState payload) : base(payload, DataTypes.DataValue)
    {
    }
}

public record EntityHandle : ManagedHandle<BaseObjectState>
{
    public EntityHandle(BaseObjectState Value) : base(Value, DataTypes.ObjectNode)
    {
    }
}


