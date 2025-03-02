using FluentResults;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal class EntityHandleManager(IEntityNode entityNode) : IEntityHandleManager
{
    private readonly HandleCollection _handles = new(entityNode);

    /// <inheritdoc />
    public bool IsHandleToAnyRelatedNode(object? handle)
    {
        if (handle == null) return false;
        return IsManagedEntityHandle(handle) || IsManagedPropertyHandle(handle, out _);
    }

    /// <inheritdoc />
    public Result<BaseInstanceState> GetState(NodeId nodeId)
    {
        if (IsManagedEntityHandle(nodeId)) return entityNode.Entity;
        if (IsManagedPropertyHandle(nodeId, out var property)) return property.Value;

        return Result.Fail($"Entity {entityNode.Entity.BrowseName} does not have any data for state {nodeId}");
    }


    public bool IsManagedPropertyHandle(object? handle, out IEntityNodeHandle managedPropertyHandle)
    {
        if (handle is PropertyHandle p
            && entityNode.PropertyStates.TryGetValue(p.Value.NodeId, out _))
        {
            managedPropertyHandle = p;
            return true;
        }

        managedPropertyHandle = null!;
        return false;
    }

    /// <inheritdoc />
    public bool IsManagedPropertyHandle(NodeId id, out IEntityNodeHandle managedPropertyHandle)
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
        if (IsManagedPropertyHandle(nodeId, out var propertyHandle)) return Result.Ok(propertyHandle);
        if (IsManagedEntityHandle(nodeId, out var entityHandle)) return Result.Ok(entityHandle);

        return Result.Fail($"Entity {entityNode.Entity.BrowseName} does not have any data for state handle {nodeId}");
    }


    public bool IsManagedEntityHandle(NodeId id, out IEntityNodeHandle entityHandle)
    {
        if (IsManagedEntityHandle(id) || id.Equals(entityNode.Entity.NodeId))
        {
            entityHandle = _handles.ManagedHandle;
            return true;
        }

        entityHandle = null!;
        return false;
    }

    public bool IsManagedEntityHandle(object? handle)
    {
        return handle is IEntityNodeHandle entityHandle && entityNode.Entity.NodeId.Equals(entityHandle.Value.NodeId);
    }
}