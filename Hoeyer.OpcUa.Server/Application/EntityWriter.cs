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
        foreach (var toWrite in nodesToWrite)
        {
            if (toWrite.AttributeId != Attributes.Value) yield return EntityWriteResponse.AttributeNotSupported(toWrite);
            
            if (entityNode.PropertyStates.TryGetValue(toWrite.NodeId, out var property))
            {
                yield return Write(toWrite, property);
            }
            else
            {
                yield return EntityWriteResponse.NoMatch(toWrite);
            }
        }
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
            return EntityWriteResponse.AssignmentFailure(nodeToWrite, propertyState);
        }
    }
}