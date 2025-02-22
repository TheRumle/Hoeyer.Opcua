using System.Linq;
using FluentResults;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;


internal class EntityHandleManager(IEntityNode entityNode) : IEntityHandleManager
{
    
    /// <inheritdoc />
    public bool IsHandleToAnyRelatedNode(object? handle)
    {
        if (handle == null) return false;
        return IsEntityHandle(handle) || IsFolderHandle(handle) || IsPropertyHandle(handle, out var _);
    }
    
    /// <inheritdoc />
    public Result<BaseInstanceState> GetState(NodeId nodeId)
    {
        if (IsEntityHandle(nodeId))
        {
            return entityNode.Entity;
        }
        if (IsFolderHandle(nodeId))
        {
            return entityNode.Folder;
        }
        if (IsPropertyHandle(nodeId, out var property))
        {
            return property.Value;
        }

        return Result.Fail($"Entity {entityNode.Entity.BrowseName} does not have any data for state {nodeId}");
    }



    public bool IsPropertyHandle(object? handle, out EntityNodeHandle<PropertyState> propertyHandle )
    {
        if (handle == null)
        {
            propertyHandle = null!;
            return false;
        }

        var prop = entityNode.PropertyStates.Values.FirstOrDefault(property => IsHandleOfProperty(handle, property));
        if (prop is not null)
        {
            propertyHandle = new EntityNodeHandle<PropertyState>(prop, entityNode.Entity);
            return true;
        } 
        
        propertyHandle = null!;
        return false;
    }

    /// <inheritdoc />
    public bool IsPropertyHandle(NodeId id, out EntityNodeHandle<PropertyState> propertyHandle)
    {
        if (entityNode.PropertyStates.TryGetValue(id, out var property))
        {
            propertyHandle = new EntityNodeHandle<PropertyState>(property, entityNode.Entity);
            return true;
        }

        propertyHandle = null!;
        return false;
    }

    /// <inheritdoc />
    public Result<EntityNodeHandle<BaseInstanceState>> GetHandle(NodeId nodeId)
    {
        if (IsPropertyHandle(nodeId, out var propertyHandle))
        {
            return Result.Ok(new EntityNodeHandle<BaseInstanceState>(propertyHandle.Value, propertyHandle.Root));
        }
        if (IsEntityHandle(nodeId, out var entityHandle))
        {
            return Result.Ok(new EntityNodeHandle<BaseInstanceState>(entityHandle.Value, entityHandle.Root));
        }
        if (IsFolderHandle(nodeId, out var folderHandle))
        {
            return Result.Ok(new EntityNodeHandle<BaseInstanceState>(folderHandle.Value, folderHandle.Root));
        }

        return Result.Fail($"Entity {entityNode.Entity.BrowseName} does not have any data for state handle {nodeId}");
    }

    
    public bool IsEntityHandle(NodeId id, out EntityNodeHandle<BaseObjectState> entityHandle)
    {
        if (IsEntityHandle(id))
        {
            entityHandle = new EntityNodeHandle<BaseObjectState>(entityNode.Entity, entityNode.Entity.Parent);
            return true;
        }

        entityHandle = null!;
        return false;
    }

    public bool IsEntityHandle(object? handle)
    {
        if (handle is null) return false;
        return handle is NodeId id && (id.Equals(entityNode.Entity.NodeId) || id.Equals(entityNode.Entity.Handle));
    }

    public bool IsFolderHandle(object? handle)
    {
        if (handle is null) return false;
        return handle is NodeId id && (id.Equals(entityNode.Folder.NodeId) || id.Equals(entityNode.Folder.Handle));
    }
    
    /// <inheritdoc />
    public bool IsFolderHandle(NodeId  id, out EntityNodeHandle<FolderState> folderHandle)
    {
        if (IsFolderHandle(id))
        {
            folderHandle = new EntityNodeHandle<FolderState>(entityNode.Folder, entityNode.Folder.Parent);
            return true;
        }
        folderHandle = null!;
        return false;
    }
    
    private static bool IsHandleOfProperty(object? handle, PropertyState propertyState)
    {
        return handle is NodeId id && (id.Equals(propertyState.NodeId) || id.Equals(propertyState.Handle));
    }

}