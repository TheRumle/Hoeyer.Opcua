using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Exceptions;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Application;

internal class EntityReferenceLinker(IEntityNode entityNode, ILogger logger) : IReferenceLinker
{
    /// <summary>
    ///     Initializes the nodes of the Entity and links nodes to existing references - for instance, making the folder lie
    ///     under the Root/Objects of the OpcUaServer
    /// </summary>
    /// <param name="externalReferences"></param>
    /// <returns>
    ///     An OK result if the operation is successful and a failed on if any operation throws or it is not possible to
    ///     link the references
    /// </returns>
    public Result InitializeToExternals(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        
        var tryGetId = (PropertyState s) => s.DataType.Identifier is uint i
            ? DataTypes.GetBrowseName((int)i)
            : "custom datatype";
        
        logger.LogInformation("Adding child nodes {@PropertiesName}",
            entityNode.PropertyStates.Values.Select(e => $"[{e.BrowseName} ({tryGetId.Invoke(e)})]"));
        
        var res = Result.Try(
            () => LinkEntity(externalReferences),
            e => new Error(e.Message));

        if (res.IsFailed) throw new UnableToInitializeException("Unable to initialize " + entityNode.BaseObject.BrowseName.Name);
        return res;
        
    }

    public Result AddReferencesToEntity(NodeId nodeId, IEnumerable<IReference> references)
    {
        var result = references.Where(e =>
            !entityNode.BaseObject.ReferenceExists(e.ReferenceTypeId, e.IsInverse, e.TargetId));
        return AddReferenceToNode(result, entityNode.BaseObject);
    }

    public Result RemoveReference(
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId)
    {
        return entityNode.BaseObject.RemoveReference(referenceTypeId, isInverse, targetId)
            ? Result.Ok()
            : Result.Fail(
                $"The managed Entity {entityNode.BaseObject.BrowseName} does not hold a reference with id {targetId}.");
    }

    private void LinkEntity(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        var refs = externalReferences.GetOrAdd(ObjectIds.ObjectsFolder, []); //fallback value
        refs.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, entityNode.BaseObject));
        entityNode.BaseObject.EventNotifier = EventNotifiers.SubscribeToEvents;
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