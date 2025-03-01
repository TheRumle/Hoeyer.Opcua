using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operational;

internal class EntityReader(IEntityNode entityNode,  ServerSystemContext context) : IEntityReader
{
    public IEnumerable<EntityValueReadResponse> ReadProperties(IEnumerable<ReadValueId> valuesToRead)
    {
        return valuesToRead.Select(Read);
    }

    public EntityValueReadResponse Read(ReadValueId toRead)
    {
        var contextCopy = context.Copy();
        if (entityNode.PropertyStates.TryGetValue(toRead.NodeId, out var propertyHandle))
        {
            return ReadProperty(toRead, propertyHandle, contextCopy);
        }
        if (entityNode.Entity.NodeId.Equals(toRead.NodeId))
        {
            return ReadEntity(toRead);
        }
        
        return new EntityValueReadResponse(toRead, $"The entity {entityNode.Entity.DisplayName} does not have any property with id {toRead.NodeId}");
    }

    private static EntityValueReadResponse ReadProperty(ReadValueId toRead, PropertyState node, ISystemContext systemContext)
    {
        return new EntityValueReadResponse(toRead, () =>
        {
            var value = new DataValue();
            value.Value = node.Value;
            var status = node.ReadAttribute(systemContext, toRead.AttributeId,
                toRead.ParsedIndexRange,
                toRead.DataEncoding, value);
            
            return (value, status.StatusCode);
        });
    }

    private EntityValueReadResponse ReadEntity(ReadValueId readId)
    {
        var dataValue = new DataValue();
        var node = entityNode.Entity;
        
        return readId.AttributeId switch
        {
            Attributes.BrowseName => new EntityValueReadResponse(readId,  () => AssignValue(dataValue, node.BrowseName)),
            Attributes.NodeClass => new EntityValueReadResponse(readId,  () => AssignValue(dataValue, (int)NodeClass.Object)),
            Attributes.DisplayName => new EntityValueReadResponse(readId, () => AssignValue(dataValue, node.DisplayName)),
            Attributes.Description => new EntityValueReadResponse(readId, () => AssignValue(dataValue, new LocalizedText($"The managed entity '{node.DisplayName.ToString()}'"))),
            _ => new EntityValueReadResponse(readId, $"Unsupported attribute ({Attributes.GetBrowseName(readId.AttributeId)}).")
        };
    }

    private static (DataValue dataValue, StatusCode Good) AssignValue(DataValue dataValue, object value)
    {
        dataValue.StatusCode = StatusCodes.Good;
        dataValue.Value = value;
        return (dataValue, StatusCodes.Good);
    }
    
}