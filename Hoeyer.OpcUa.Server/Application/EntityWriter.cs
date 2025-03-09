using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

/// <summary>
///     Edits entities. Handles modification of an Entity.
/// </summary>
internal class EntityWriter(IEntityNode entityNode, Func<ISystemContext> contextProvider) : IEntityWriter
{
    public IEnumerable<EntityWriteResponse> Write(IEnumerable<WriteValue> nodesToWrite)
    {
        //TODO fix - BAD USER ACCESS DENIED (just assign directly!)
        foreach (var toWrite in nodesToWrite)
        {
            if (entityNode.PropertyStates.TryGetValue(toWrite.NodeId, out var property))
            {
                yield return Write(toWrite, property);
                continue;
            }

            yield return new EntityWriteResponse(toWrite, StatusCodes.BadNoMatch,
                $"Cannot read node {entityNode.Entity.BrowseName} NodeId {toWrite.NodeId} as it is not related to the entity!");
        }
    }

    private EntityWriteResponse Write(WriteValue nodeToWrite, PropertyState propertyState)
    {
        var context = contextProvider.Invoke();
        var writeResult = propertyState.WriteAttribute(context,
            nodeToWrite.AttributeId,
            nodeToWrite.ParsedIndexRange,
            nodeToWrite.Value);

        var writeR = writeResult ?? StatusCodes.BadWriteNotSupported;
        return ServiceResult.IsGood(writeResult)
            ? new EntityWriteResponse(nodeToWrite, writeR.StatusCode)
            : new EntityWriteResponse(nodeToWrite, writeR, $"Could not assign {entityNode.Entity.BrowseName}.{propertyState.BrowseName} to value {nodeToWrite.Value}.");
    }
}