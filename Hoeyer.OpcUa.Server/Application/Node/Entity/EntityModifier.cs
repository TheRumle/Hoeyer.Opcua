using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions.Exceptions;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.Node.Entity.Exceptions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity;

/// <summary>
/// Edits entities. Handles modification of an Entity. 
/// </summary>
internal class EntityModifier(EntityNode entityNode, EntityHandleManager handleManager, ILogger? logger = null)
{
    private BaseObjectState Entity => entityNode.Entity;
    
    public EntityModifier(EntityNode entityNode, ILogger? logger = null)
        : this(entityNode, new EntityHandleManager(entityNode, logger), logger)
    {}
    
    public void AddReferences(IDictionary<NodeId, IList<IReference>> references)
    {
        if (references.Count == 0) return;
        foreach (var r in references[Entity.NodeId])
        {
            Entity.AddReference(r.ReferenceTypeId, r.IsInverse, r.TargetId);
        }
    }
    
    
    public ServiceResult DeleteReference(
        object sourceHandle,
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId)
    {
        if(!handleManager.IsEntityHandle(sourceHandle)) throw new NoSuchManagedNodeException(entityNode, sourceHandle);
        if (!Entity.ReferenceExists(referenceTypeId, isInverse, targetId))
        { 
            var e = new NoSuchEntityFieldException(entityNode, targetId);
            logger?.LogError(e, "Entity '{@SourceHandle}' was not referenced by this entity is not a valid field in {@Entity}", sourceHandle, Entity);
            throw e;
        }

        Entity.RemoveReference(referenceTypeId, isInverse, targetId);
        logger?.LogInformation("Removed reference with ReferenceId {@ReferenceTypeId} from Entity: {@Entity}", referenceTypeId,
            Entity.DisplayName);
        return ServiceResult.Good;
    }

    public void Write(ServerSystemContext systemContext, IList<WriteValue> nodesToWrite)
    {
           
        
        
        foreach (var nodeToWrite in nodesToWrite)
        {
            if (nodeToWrite.Processed) continue;
            
            var resultCode = Entity.WriteAttribute(systemContext,
                nodeToWrite.AttributeId,
                nodeToWrite.ParsedIndexRange,
                nodeToWrite.Value);
            
            nodeToWrite.Processed = true;

            logger?.LogInformation("Wrote value {@Value} to entity {@Entity} with result code {@ResultCode}", nodeToWrite.Value, Entity.BrowseName, resultCode);
        }
    }
}