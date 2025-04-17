using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Hoeyer.OpcUa.Server.Entity.Application.Handle;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Application;

internal class EntityBrowser(IEntityNode node) : IEntityBrowser
{
    /// <inheritdoc />
    public Result<EntityBrowseResponse> Browse(ContinuationPoint continuationPoint,
        IEntityNodeHandle nodeToBrowse)
    {
        var browseResult = nodeToBrowse switch
        {
            EntityHandle entityHandle
                when entityHandle.Payload.Equals(node.BaseObject) => BrowseEntity(continuationPoint),

            PropertyHandle propertyHandle
                when node.PropertyStates.ContainsValue(propertyHandle.Payload) =>
                BrowseProperty(propertyHandle.Payload),

            _ => Result.Fail(
                $"{nodeToBrowse.Value.BrowseName} is not related to the entity {node.BaseObject.BrowseName}")
        };
        

        return browseResult
            .Map(values => values
                .Skip(continuationPoint.Index)
                .Take((int)Math.
                    Min(Math.Max(continuationPoint.MaxResultsToReturn, 1),
                        int.MaxValue)))
            .Map(foundValues => CreateBrowseResponse(foundValues.ToList(), continuationPoint));
    }

    private static EntityBrowseResponse CreateBrowseResponse(IList<ReferenceDescription> foundValues,
        ContinuationPoint continuationPoint)
    {
        if (foundValues.Count < continuationPoint.MaxResultsToReturn)
        {
            return new EntityBrowseResponse(null, foundValues);
        }
        return new EntityBrowseResponse(continuationPoint, foundValues);
    }


    /// <inheritdoc />
    public Result<IEnumerable<ReferenceDescription>> BrowseEntity(ContinuationPoint continuationPoint)
    {
        return Result.Ok(node.PropertyStates.Values.Select(propertyState =>
            CreateDescription(propertyState, continuationPoint)));
    }

    private static Result<IEnumerable<ReferenceDescription>> BrowseProperty(PropertyState managed)
    {
        IEnumerable<ReferenceDescription> descriptions =
        [
            new()
            {
                ReferenceTypeId = managed.ReferenceTypeId,
                NodeId = managed.NodeId,
                BrowseName = managed.BrowseName,
                DisplayName = managed.DisplayName,
                NodeClass = NodeClass.Variable,
                TypeDefinition = managed.TypeDefinitionId
            }
        ];
        return Result.Ok(descriptions);
    }


    private static ReferenceDescription CreateDescription(PropertyState propertyState,
        ContinuationPoint continuationPoint)
    {
        var description = new ReferenceDescription
        {
            NodeId = propertyState.NodeId
        };
        var resultMask = continuationPoint.ResultMask;
        description.SetReferenceType(resultMask, propertyState.ReferenceTypeId, false);
        description.SetTargetAttributes(resultMask, propertyState.NodeClass, propertyState.BrowseName,
            propertyState.DisplayName, propertyState.TypeDefinitionId);
        return description;
    }
}