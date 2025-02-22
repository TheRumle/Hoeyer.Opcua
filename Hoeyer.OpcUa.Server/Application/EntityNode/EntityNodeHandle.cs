using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public record EntityNodeHandle<T>(T Value, NodeState Root) where T : NodeState
{
    internal readonly T Value = Value;
    internal readonly NodeState Root = Root;

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