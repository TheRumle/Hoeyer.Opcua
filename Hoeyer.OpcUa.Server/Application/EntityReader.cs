using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Application.RequestResponse;
using Hoeyer.OpcUa.Server.Entity.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal class EntityReader(IEntityNode entityNode, IPropertyReader reader) : IEntityReader
{
    public IEnumerable<EntityValueReadResponse> ReadProperties(IEnumerable<ReadValueId> valuesToRead)
    {
        return valuesToRead.Select(Read);
    }

    private EntityValueReadResponse Read(ReadValueId toRead)
    {
        if (entityNode.PropertyStates.TryGetValue(toRead.NodeId, out var propertyHandle))
            return reader.ReadProperty(toRead, propertyHandle);
        if (entityNode.Entity.NodeId.Equals(toRead.NodeId)) return ReadEntity(toRead);

        return new EntityValueReadResponse(toRead, StatusCodes.BadNoEntryExists,
            $"The entity {entityNode.Entity.DisplayName} does not have any property with id {toRead.NodeId}");
    }

    private EntityValueReadResponse ReadEntity(ReadValueId readId)
    {
        var node = entityNode.Entity;
        return readId.AttributeId switch
        {
            Attributes.BrowseName => new EntityValueReadResponse(readId, () => AssignValue(node.BrowseName)),
            Attributes.NodeClass => new EntityValueReadResponse(readId, () => AssignValue((int)NodeClass.Object)),
            Attributes.DisplayName => new EntityValueReadResponse(readId, () => AssignValue(node.DisplayName)),
            Attributes.Description => new EntityValueReadResponse(readId,
                () => AssignValue(new LocalizedText($"The managed entity '{node.DisplayName.ToString()}'"))),
            Attributes.NodeId => new EntityValueReadResponse(readId, () => AssignValue(node.NodeId)),
            _ => new EntityValueReadResponse(readId, StatusCodes.BadNotSupported)
        };
    }


    private static (DataValue dataValue, StatusCode Good) AssignValue(object value)
    {
        var dataValue = new DataValue();
        dataValue.StatusCode = StatusCodes.Good;
        dataValue.Value = value;
        return (dataValue, StatusCodes.Good);
    }
}