using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operational;

internal class EntityReferenceLinker(IEntityNode entityNode) : IReferenceLinker
{
    /// <summary>
    /// Initializes the nodes of the Entity and links nodes to existing references - for instance, making the folder lie under the Root/Objects of the OpcUaServer
    /// </summary>
    /// <param name="externalReferences"></param>
    /// <returns>An OK result if the operation is successful and a failed on if any operation throws or it is not possible to link the references</returns>
    public Result IntitializeNodeWithReferences(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        return Result.Try(
            () => LinkEntity(externalReferences),
            e  => new Error(e.Message));
    }

    private void LinkEntity(IDictionary<NodeId, IList<IReference>> externalReferences)
    {

        externalReferences.GetOrAdd(ObjectIds.ObjectsFolder,
        [
            new NodeStateReference(ReferenceTypeIds.Organizes, false, entityNode.Entity),
        ]);

        entityNode.Entity.EventNotifier = EventNotifiers.SubscribeToEvents;

        foreach (var propertyState in entityNode.PropertyStates.Values)
        {
            entityNode.Entity.AddReference(ReferenceTypes.HasProperty, false, propertyState.NodeId);
        }
    }

    public Result AddReferencesToEntity(NodeId nodeId, IEnumerable<IReference> references)
    {
        var result = references.Where(e => !entityNode.Entity.ReferenceExists(e.ReferenceTypeId, e.IsInverse, e.TargetId));
        return AddReferenceToNode( result, entityNode.Entity);
    }

    public Result RemoveReference(
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
            nodeState.AddReference(r.ReferenceTypeId, r.IsInverse, r.TargetId);
            return Result.Ok();
        }).Merge();
    }
}