using System.Collections.Generic;
using FluentResults;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Application.RequestResponse;
using Hoeyer.OpcUa.Server.Entity;
using Hoeyer.OpcUa.Server.Entity.Api;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

/// <summary>
///     Edits entities. Handles modification of an Entity.
/// </summary>
internal class EntityWriter(IEntityNode entityNode) : IEntityWriter
{
    public IEnumerable<EntityWriteResponse> Write(IEnumerable<WriteValue> nodesToWrite, ISystemContext context)
    {
        foreach (var toWrite in nodesToWrite)
        {
            if (entityNode.PropertyStates.TryGetValue(toWrite.NodeId, out var property))
            {
                yield return Write(toWrite, property, context);
                continue;
            }

            yield return new EntityWriteResponse(toWrite, StatusCodes.BadNoMatch, $"Cannot read node {entityNode.Entity.BrowseName} NodeId {toWrite.NodeId} as it is not related to the entity!");
        }
    }

    private EntityWriteResponse Write(WriteValue nodeToWrite, PropertyState propertyState, ISystemContext context)
    {
        var writeResult = propertyState.WriteAttribute(context,
            nodeToWrite.AttributeId,
            nodeToWrite.ParsedIndexRange,
            nodeToWrite.Value);

        var writeR = writeResult ?? StatusCodes.BadWriteNotSupported;
        return StatusCode.IsGood(writeR.StatusCode)
            ? new EntityWriteResponse(nodeToWrite, writeR.StatusCode)
            : new EntityWriteResponse(nodeToWrite, writeR ,$"Could not assign {entityNode.Entity.BrowseName}.{propertyState.BrowseName} to value {nodeToWrite.Value}.");
    }
}