using System;
using System.Collections.Generic;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Application;

/// <summary>
///     Edits entities. Handles modification of an Entity.
/// </summary>
internal class EntityStateChanger(IEntityNode entityNode, IMessagePublisher<IEntityNode> broadcastChange) : IEntityWriter
{
    public IEnumerable<EntityWriteResponse> Write(IEnumerable<WriteValue> nodesToWrite)
    {
        var result = ProcessWriteRequests(nodesToWrite, out var stateChanged);
        if (stateChanged) broadcastChange.Publish(entityNode);
        return result;
    }

    private IEnumerable<EntityWriteResponse> ProcessWriteRequests(IEnumerable<WriteValue> nodesToWrite, out bool stateChange)
    {
        stateChange = false;
        List<EntityWriteResponse> result = new List<EntityWriteResponse>();
        foreach (var toWrite in nodesToWrite)
        {
            if (toWrite.AttributeId != Attributes.Value)
            {
                result.Add(EntityWriteResponse.AttributeNotSupported(toWrite));
            }

            if (entityNode.PropertyStates.TryGetValue(toWrite.NodeId, out var property))
            {
                result.Add(Write(toWrite, property));
                stateChange = true;
            }
            else
            {
                result.Add(EntityWriteResponse.NoMatch(toWrite));
            }
        }

        return result;
    }

    private static EntityWriteResponse Write(WriteValue nodeToWrite, PropertyState propertyState)
    {
        try
        {
            propertyState.Value = nodeToWrite.Value.Value;
            return new EntityWriteResponse(nodeToWrite, StatusCodes.Good);
        }
        catch (Exception e)
        {
            return EntityWriteResponse.AssignmentFailure(nodeToWrite, propertyState, e);
        }
    }
}