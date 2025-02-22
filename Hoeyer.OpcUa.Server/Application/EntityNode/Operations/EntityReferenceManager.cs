using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

internal class EntityReferenceManager(IEntityNode entityNode) : IEntityReferenceManager
{
    public Result IntitializeNodeWithReferences(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        // Add to objects folder  EntityFolder <--> Opc.Objects
        // EntityFolder <--> Entity
        externalReferences.GetOrAdd(ObjectIds.ObjectsFolder,
            [
                ..GetReferenceEdge(ReferenceTypeIds.Organizes, entityNode.Folder).ToEnumerable(),
                ..GetReferenceEdge(ReferenceTypeIds.Organizes, entityNode.Entity).ToEnumerable()
            ]);

        entityNode.Entity.EventNotifier = EventNotifiers.SubscribeToEvents;
        // Entity <--> Property
        foreach (var propertyState in entityNode.PropertyStates.Values)
        {
            AddPropertyReferences(ReferenceTypes.HasProperty, propertyState);
        }
        return Result.Ok();
    }
    
    public Result AddReferencesToEntity(IEnumerable<IReference> references)
    {
        return AddReferenceToNode(references, entityNode.Entity);
    }
    
    public Result AddReferencesToFolder(IEnumerable<IReference> references)
    {
        return AddReferenceToNode(references, entityNode.Folder);
    }

    public Result DeleteReference(
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId)
    {
        return entityNode.Entity.RemoveReference(referenceTypeId, isInverse, targetId) 
            ? Result.Ok() 
            : Result.Fail($"The managed Entity {entityNode.Entity.BrowseName} does not hold a reference with id {targetId}.");
    }
    
    private static Result AddReferenceToNode(IEnumerable<IReference> references, NodeState nodeState)
    {
        return references.Select(r =>
        {
            if (nodeState.ReferenceExists(r.ReferenceTypeId, r.IsInverse, r.TargetId))
            {
                return Result.Fail($"Reference '{r.ReferenceTypeId}' already exists on {nodeState.BrowseName}");
            }
            nodeState.AddReference(r.ReferenceTypeId, r.IsInverse, r.TargetId);
            return Result.Ok();
        }).Merge();

    }

    /// <summary>
    /// Add reference edge A <--> B
    /// </summary>
    private static (NodeStateReference, NodeStateReference) GetReferenceEdge(NodeId referenceTypeId, NodeState node)
    {
        return (new(referenceTypeId, true, node), new(referenceTypeId, false, node));
    }

    private void AddPropertyReferences(NodeId referenceId, PropertyState child)
    {
        var parent = entityNode.Entity;
        parent.AddReference(referenceId, false, child.NodeId);
        child.AddReference(referenceId, true, parent.NodeId);
        parent.AddChild(child);
    }
}