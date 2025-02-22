using FluentResults;
using Hoeyer.OpcUa.Server.Application.EntityNode.Operations;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public class EntityNodeHandle<T>(T value, NodeState root) where T : NodeState
{
    internal readonly T Value = value;
    internal readonly NodeState Root = root;

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


public interface IEntityHandleManager
{  
    public bool IsHandleToAnyRelatedNode(object? handle);
    
    public bool IsPropertyHandle(object? handle, out EntityNodeHandle<PropertyState> propertyHandle);
    public bool IsPropertyHandle(NodeId id, out EntityNodeHandle<PropertyState> propertyHandle);
    public bool IsFolderHandle(object? handle);
    public bool IsFolderHandle(NodeId id,  out EntityNodeHandle<FolderState> folderHandle);
    public bool IsEntityHandle(object? handle);
    public bool IsEntityHandle(NodeId id, out EntityNodeHandle<BaseObjectState> entityHandle);

    /// <summary>
    /// Gets the handle object of property, entity, or folder if it exists
    /// </summary>
    /// <returns>A result with the <see cref="BaseInstanceState"/> of the entity itself, the folder containing the entity or a property of the entity.</returns>
    public Result<EntityNodeHandle<BaseInstanceState>> GetHandle(NodeId nodeId);
    public Result<BaseInstanceState> GetState(NodeId nodeId);

}