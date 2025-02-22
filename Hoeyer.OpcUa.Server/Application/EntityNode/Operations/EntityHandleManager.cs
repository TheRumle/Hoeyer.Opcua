using FluentResults;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

internal class EntityHandleManager(IEntityNode entityNode) : IEntityHandleManager
{
    private readonly HandleCollection _handles = new(entityNode);

    /// <inheritdoc />
    public bool IsHandleToAnyRelatedNode(object? handle)
    {
        if (handle == null) return false;
        return IsManagedEntityHandle(handle) || IsManagedFolderHandle(handle) || IsManagedPropertyHandle(handle, out _);
    }

    /// <inheritdoc />
    public Result<BaseInstanceState> GetState(NodeId nodeId)
    {
        if (IsManagedEntityHandle(nodeId)) return entityNode.Entity;
        if (IsManagedFolderHandle(nodeId)) return entityNode.Folder;
        if (IsManagedPropertyHandle(nodeId, out var property)) return property.Value;

        return Result.Fail($"Entity {entityNode.Entity.BrowseName} does not have any data for state {nodeId}");
    }


    public bool IsManagedPropertyHandle(object? handle, out EntityNodeHandle<PropertyState> managedPropertyHandle)
    {
        if (handle is EntityNodeHandle<PropertyState> p
            && entityNode.PropertyStates.TryGetValue(p.Value.NodeId, out _))
        {
            managedPropertyHandle = p;
            return true;
        }

        managedPropertyHandle = null!;
        return false;
    }

    /// <inheritdoc />
    public bool IsManagedPropertyHandle(NodeId id, out EntityNodeHandle<PropertyState> managedPropertyHandle)
    {
        if (entityNode.PropertyStates.TryGetValue(id, out var property))
        {
            managedPropertyHandle = _handles.GetOrCreatePropertyHandle(property);
            return true;
        }

        managedPropertyHandle = null!;
        return false;
    }

    /// <inheritdoc />
    public Result<IEntityNodeHandle> GetHandle(NodeId nodeId)
    {
        if (IsManagedPropertyHandle(nodeId, out var propertyHandle))
            return Result.Ok<IEntityNodeHandle>(propertyHandle);
        if (IsManagedEntityHandle(nodeId, out var entityHandle)) return Result.Ok<IEntityNodeHandle>(entityHandle);
        if (IsManagedFolderHandle(nodeId, out var folderHandle)) return Result.Ok<IEntityNodeHandle>(folderHandle);

        return Result.Fail($"Entity {entityNode.Entity.BrowseName} does not have any data for state handle {nodeId}");
    }


    public bool IsManagedEntityHandle(NodeId id, out EntityNodeHandle<BaseObjectState> entityHandle)
    {
        if (IsManagedEntityHandle(id) || id.Equals(entityNode.Entity.NodeId))
        {
            entityHandle = _handles.EntityNodeHandle;
            return true;
        }

        entityHandle = null!;
        return false;
    }

    public bool IsManagedEntityHandle(object? handle)
    {
        return handle is EntityNodeHandle<BaseObjectState> entityHandle &&
               entityNode.Entity.NodeId.Equals(entityHandle.Value.NodeId);
    }

    public bool IsManagedFolderHandle(object? handle)
    {
        return handle is EntityNodeHandle<FolderState> folderHandle &&
               entityNode.Folder.NodeId.Equals(folderHandle.Value.NodeId);
    }

    /// <inheritdoc />
    public bool IsManagedFolderHandle(NodeId id, out EntityNodeHandle<FolderState> folderHandle)
    {
        if (IsManagedFolderHandle(id) || id.Equals(entityNode.Folder.NodeId))
        {
            folderHandle = _handles.FolderHandle;
            return true;
        }

        folderHandle = null!;
        return false;
    }
}