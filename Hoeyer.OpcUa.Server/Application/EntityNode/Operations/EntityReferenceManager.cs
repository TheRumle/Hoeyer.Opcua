using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

internal class EntityReferenceManager(IEntityNode entityNode) : IEntityReferenceManager
{
    /// <summary>
    /// Initializes the nodes of the Entity and links nodes to existing references - for instance, making the folder lie under the Root/Objects of the OpcUaServer
    /// </summary>
    /// <param name="externalReferences"></param>
    /// <returns></returns>
    public Result IntitializeNodeWithReferences(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        externalReferences.GetOrAdd(ObjectIds.RootFolder,
        [
            new NodeStateReference(ReferenceTypeIds.Organizes, false, entityNode.Entity),
        ]);
        
        entityNode.Folder.AddReference(ReferenceTypeIds.Organizes, false, new ExpandedNodeId(entityNode.Entity.NodeId));

        entityNode.Entity.EventNotifier = EventNotifiers.SubscribeToEvents;
        entityNode.Folder.EventNotifier = EventNotifiers.SubscribeToEvents;

        // Entity <--> Property
        foreach (var propertyState in entityNode.PropertyStates.Values)
            AddPropertyReferences(ReferenceTypes.HasProperty, propertyState);
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
            : Result.Fail(
                $"The managed Entity {entityNode.Entity.BrowseName} does not hold a reference with id {targetId}.");
    }

    private static Result AddReferenceToNode(IEnumerable<IReference> references, NodeState nodeState)
    {
        return references.Select(r =>
        {
            if (nodeState.ReferenceExists(r.ReferenceTypeId, r.IsInverse, r.TargetId))
                return Result.Fail($"Reference '{r.ReferenceTypeId}' already exists on {nodeState.BrowseName}");
            nodeState.AddReference(r.ReferenceTypeId, r.IsInverse, r.TargetId);
            return Result.Ok();
        }).Merge();
    }

    /// <summary>
    ///     Add reference edge A <--> B
    /// </summary>
    private static (NodeStateReference, NodeStateReference) GetReferenceEdge(NodeId referenceTypeId, NodeState node)
    {
        return (new NodeStateReference(referenceTypeId, true, node),
            new NodeStateReference(referenceTypeId, false, node));
    }

    private void AddPropertyReferences(NodeId referenceId, PropertyState child)
    {
        var parent = entityNode.Entity;
        parent.AddReference(referenceId, false, child.NodeId);
        if (!parent.ReferenceExists(referenceId, false, new ExpandedNodeId(child.NodeId)))
        {
            parent.AddChild(child);
        }
    }
}