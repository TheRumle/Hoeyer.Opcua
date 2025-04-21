using System;
using System.Collections.Generic;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

/// <summary>
///     Edits entities. Handles modification of an Entity.
/// </summary>
internal class EntityStateChanger(IEntityNode entityNode, IEntityChangedBroadcaster broadcastChange) : IEntityWriter
{
    public IEnumerable<EntityWriteResponse> Write(IEnumerable<WriteValue> nodesToWrite)
    {
        var result = ProcessWriteRequests(nodesToWrite, out var changes);
        if (changes.Count > 0) broadcastChange.Publish((entityNode, changes));
        return result;
    }

    private IEnumerable<EntityWriteResponse> ProcessWriteRequests(IEnumerable<WriteValue> nodesToWrite, out List<StateChange<PropertyState, object>> changes)
    {
        changes = new();
        var result = new List<EntityWriteResponse>();
        foreach (var toWrite in nodesToWrite)
        {
            if (toWrite.AttributeId != Attributes.Value)
            {
                result.Add(EntityWriteResponse.AttributeNotSupported(toWrite));
            }

            if (entityNode.PropertyStates.TryGetValue(toWrite.NodeId, out var property))
            {
                var oldValue = property.Value;
                result.Add(Write(toWrite, property));
                changes.Add(new StateChange<PropertyState, object>(property, oldValue, property.Value ));
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