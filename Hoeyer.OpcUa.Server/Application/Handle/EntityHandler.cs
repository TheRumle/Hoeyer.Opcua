using System.Collections.Generic;
using FluentResults;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Handle;

internal sealed class EntityHandler(IEntityNode entityNode) : IEntityHandleManager
{
    private readonly HandleCollection _handles = new(entityNode);

    /// <inheritdoc />
    public IEntityNodeHandle EntityHandle => _handles.ManagedHandle;

    /// <inheritdoc />
    public IEnumerable<IEntityNodeHandle> PropertyHandles => _handles.PropertyHandles.Values;

    /// <inheritdoc />
    public bool IsHandleToAnyRelatedNode(object? handle)
    {
        if (handle == null)
        {
            return false;
        }

        return IsManagedEntityHandle(handle) || IsManagedPropertyHandle(handle, out _);
    }

    /// <inheritdoc />
    public Result<BaseInstanceState> GetState(NodeId nodeId)
    {
        if (IsManagedEntityHandle(nodeId))
        {
            return entityNode.BaseObject;
        }

        if (IsManagedPropertyHandle(nodeId, out var property))
        {
            return property.Value;
        }

        return Result.Fail($"Entity {entityNode.BaseObject.BrowseName} does not have any data for state {nodeId}");
    }

    /// <inheritdoc />
    public bool IsManaged(NodeId nodeId)
    {
        if (entityNode.BaseObject.NodeId.Equals(nodeId))
        {
            return true;
        }

        if (entityNode.PropertyStates.ContainsKey(nodeId))
        {
            return true;
        }

        return false;
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
        if (IsManagedPropertyHandle(nodeId, out var propertyHandle))
        {
            return Result.Ok(propertyHandle);
        }

        if (TryGetEntityHandle(nodeId, out var entityHandle))
        {
            return Result.Ok(entityHandle);
        }

        return Result.Fail(
            $"Entity {entityNode.BaseObject.BrowseName} does not have any data for state handle {nodeId}");
    }


    public bool TryGetEntityHandle(NodeId id, out IEntityNodeHandle entityHandle)
    {
        if (IsManagedEntityHandle(id) || id.Equals(entityNode.BaseObject.NodeId))
        {
            entityHandle = _handles.ManagedHandle;
            return true;
        }

        entityHandle = null!;
        return false;
    }

    public bool IsManagedEntityHandle(object? handle)
    {
        return entityNode.BaseObject.NodeId.Equals(handle) 
            || handle is IEntityNodeHandle entityHandle &&
               entityNode.BaseObject.NodeId.Equals(entityHandle.Value.NodeId);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        entityNode.BaseObject.Dispose();
        foreach (var propertyStatesValue in entityNode.PropertyStates.Values) propertyStatesValue.Dispose();
        entityNode.PropertyStates.Clear();
    }
}