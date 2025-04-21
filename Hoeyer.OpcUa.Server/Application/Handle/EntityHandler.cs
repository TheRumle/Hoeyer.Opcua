using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Handle;

internal sealed class EntityHandler(IEntityNode entityNode) : IEntityHandleManager
{
    private readonly HandleCollection _handles = new(entityNode);

    public IEntityNodeHandle EntityHandle => _handles.ManagedHandle;

    public bool IsHandleToAnyRelatedNode(object? handle)
    {
        if (handle == null)
        {
            return false;
        }

        return IsManagedEntityHandle(handle) || IsManagedPropertyHandle(handle, out _);
    }

    /// <inheritdoc />
    public BaseInstanceState? GetState(NodeId nodeId)
    {
        if (IsManagedEntityHandle(nodeId))
        {
            return entityNode.BaseObject;
        }

        if (IsManagedPropertyHandle(nodeId, out var property))
        {
            return property.Value;
        }

        return null;
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


    public bool IsManagedPropertyHandle(object? handle, out ManagedHandle<PropertyState> managedPropertyHandle)
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
    public bool IsManagedPropertyHandle(NodeId id, out ManagedHandle<PropertyState> managedPropertyHandle)
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
    public IEntityNodeHandle? GetHandle(NodeId nodeId)
    {
        if (IsManagedPropertyHandle(nodeId, out var propertyHandle))
        {
            return propertyHandle;
        }

        if (TryGetEntityHandle(nodeId, out var entityHandle))
        {
            return entityHandle;
        }

        return null;
    }


    public bool TryGetEntityHandle(NodeId id, out ManagedHandle<BaseObjectState> entityHandle)
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