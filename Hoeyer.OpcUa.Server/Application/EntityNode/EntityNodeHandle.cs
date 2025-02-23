using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IEntityNodeHandle
{
    internal BaseInstanceState HandledNode { get; }
}

public record EntityNodeHandle<T>(T Value, NodeState Root) : IEntityNodeHandle
    where T : BaseInstanceState
{
    internal readonly NodeState Root = Root;
    internal readonly T Value = Value;

    /// <inheritdoc />
    public BaseInstanceState HandledNode => Value;

    public static implicit operator NodeHandle(EntityNodeHandle<T> entityNodeHandle)
    {
        return new NodeHandle
        {
            NodeId = entityNodeHandle.Value.NodeId,
            Validated = true,
            Node = entityNodeHandle.Value,
            RootId = entityNodeHandle.Root.NodeId
        };
    }
}