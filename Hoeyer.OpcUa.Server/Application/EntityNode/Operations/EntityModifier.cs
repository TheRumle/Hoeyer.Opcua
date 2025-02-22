using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

public interface IEntityReferenceManager
{
    Result LinkToRootNodes(IDictionary<NodeId, IList<IReference>> externalReferences);
    Result AddReferencesToEntity(IEnumerable<IReference> references);
    Result AddReferencesToFolder(IEnumerable<IReference> references);

    Result DeleteReference(
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId);
}

internal class EntityReferenceManager(IEntityNode entityNode) : IEntityReferenceManager
{
    public Result LinkToRootNodes(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        // Add add to objects folder  EntityFolder <--> Opc.Objects
        // EntityFolder <--> Entity 
        externalReferences.GetOrAdd(ObjectIds.ObjectsFolder, [new NodeStateReference(ReferenceTypeIds.Organizes, false, entityNode.Folder.NodeId)]);
        entityNode.Folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);

        // Entity --> Property
        foreach (var propertyState in entityNode.PropertyStates)
        {
            entityNode.Entity.AddReference(ReferenceTypeIds.HasProperty, false, propertyState.Key);
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

    public Result DeleteReference(
        NodeId referenceTypeId,
        bool isInverse,
        ExpandedNodeId targetId)
    {
        return entityNode.Entity.RemoveReference(referenceTypeId, isInverse, targetId) 
            ? Result.Ok() 
            : Result.Fail($"The managed Entity {entityNode.Entity.BrowseName} does not hold a reference with id {targetId}.");
    }
}

/// <summary>
/// Edits entities. Handles modification of an Entity. 
/// </summary>
internal class EntityModifier(IEntityNode entityNode) : IEntityModifier 
{

    public IEnumerable<Result<ServiceResult>> Write(ISystemContext systemContext, IEnumerable<WriteValue> nodesToWrite)
    {
        foreach (var toWrite in nodesToWrite)
        {
            if (entityNode.PropertyStates.TryGetValue(toWrite.NodeId, out var property))
            {
                yield return Write(systemContext, toWrite, property);
                continue;
            }

            yield return Result.Fail(
                $"Cannot read node {entityNode.Entity.BrowseName} NodeId {toWrite.NodeId} as it is not related to the entity!");
        }
        
    }

    private Result<ServiceResult> Write(ISystemContext systemContext, WriteValue nodeToWrite, PropertyState propertyState)
    {
        var writeResult = propertyState.WriteAttribute(systemContext,
            nodeToWrite.AttributeId,
            nodeToWrite.ParsedIndexRange,
            nodeToWrite.Value);

        return writeResult != null && writeResult.StatusCode == StatusCodes.Good
            ? Result.Ok(writeResult)
            : Result.Fail($"Could not assign {entityNode.Entity.BrowseName}.{propertyState.BrowseName} to value {nodeToWrite.Value}.");
    }
}