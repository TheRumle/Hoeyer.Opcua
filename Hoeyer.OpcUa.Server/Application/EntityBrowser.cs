using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal class EntityBrowser(IEntityNode node) : IEntityBrowser
{
    /// <inheritdoc />
    public Result<IEnumerable<ReferenceDescription>> Browse(ContinuationPoint continuationPoint,
        IEntityNodeHandle nodeToBrowse)
    {
        var result = nodeToBrowse switch
        {
            EntityHandle { Value: var managed } when managed.Equals(node.Entity) => Result.Ok(
                BrowseEntity(continuationPoint)
                    .Skip(continuationPoint.Index)
                    .Take((int)continuationPoint.MaxResultsToReturn)
            ),
            PropertyHandle { Payload: var managed } when node.PropertyStates.ContainsValue(managed) => Result.Ok(
                BrowseProperty(managed)),
            _ => Result.Fail(
                $"{nodeToBrowse.Value.DisplayName} is not associated with browser for entity {node.Entity.DisplayName}.")
        };

        return result
            .Map(values => values.ToList())
            .Then(foundValues => { continuationPoint.Index += foundValues.Count; })
            .Map(IEnumerable<ReferenceDescription> (values) => values);
    }

    /// <inheritdoc />
    public IEnumerable<ReferenceDescription> BrowseEntity(ContinuationPoint continuationPoint)
    {
        return node.PropertyStates.Values.Select(propertyState => CreateDescription(propertyState, continuationPoint));
    }

    private static IEnumerable<ReferenceDescription> BrowseProperty(PropertyState managed)
    {
        return
        [
            new ReferenceDescription
            {
                ReferenceTypeId = managed.ReferenceTypeId,
                NodeId = managed.NodeId,
                BrowseName = managed.BrowseName,
                DisplayName = managed.DisplayName,
                NodeClass = NodeClass.Variable,
                TypeDefinition = managed.TypeDefinitionId
            }
        ];
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