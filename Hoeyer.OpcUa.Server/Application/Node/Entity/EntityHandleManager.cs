using System.Collections.Generic;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.Node.Entity.Exceptions;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity;

internal class EntityHandleManager(EntityNode entityNode, ILogger? logger = null)
{
    private readonly BaseObjectState _entity = entityNode.Entity;
    private readonly IReadOnlyDictionary<NodeId, BaseObjectState> _allNodes = new Dictionary<NodeId, BaseObjectState>
    {
        [entityNode.Entity.NodeId] = entityNode.Entity,
        [entityNode.Folder.NodeId] = entityNode.Folder
    };

    /// <summary>
    /// Tries to find a managed <see cref="BaseObjectState"/> that represents an Entity or null if no matching node exists.
    /// If the NodeId is invalid, then logs a warning specifying the node. 
    /// </summary>
    /// <param name="nodeId">The id of the BaseObjectState that represents a managed Entity </param>
    /// <returns></returns>
    public BaseObjectState? GetEntityHandle(NodeId nodeId)
    {
        if (NodeId.IsNull(nodeId) || nodeId.Identifier is null)
        {
            logger?.LogWarning("Invalid NodeId. The NodeId is null or does not have an identifier. NodeId: {@NodeId}",
                nodeId);
            return null;
        }
        _allNodes.TryGetValue(nodeId, out var value);
        return value;
    }

 
    public BaseObjectState? GetNodeForHandle(object? handle)
    {
        return handle switch
        {
            null => GetEntityHandle(null!), // for logging purposes
            NodeId id => GetEntityHandle(id),
            NodeState source => GetEntityHandle(source.NodeId),
            _ => LogInvalidHandle()
        };

        BaseObjectState LogInvalidHandle()
        {
            logger?.LogError("Invalid source handle. The source handle '{@SourceHandle}' is not a valid.",
                handle);
            throw new InvalidHandleException(handle);
        }
    }

    public bool IsEntityHandle(object handle)
    {
        BaseObjectState? h = GetNodeForHandle(handle);
        return h != null && h.Equals(_entity);
    }
    
    public bool IsEntityHandle(NodeId handle)
    {
        return _entity.NodeId.Equals(handle);
    }
}