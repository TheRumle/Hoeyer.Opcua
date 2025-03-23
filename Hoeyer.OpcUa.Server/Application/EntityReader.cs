using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Opc.Ua;
using NodeClass = Opc.Ua.NodeClass;

namespace Hoeyer.OpcUa.Server.Application;

internal class EntityReader(IEntityNode entityNode, IPropertyReader propertyReader) : IEntityReader
{
    public IEnumerable<EntityValueReadResponse> ReadAttributes(IEnumerable<ReadValueId> valuesToRead)
    {
        return valuesToRead.Select(Read);
    }

    private EntityValueReadResponse Read(ReadValueId toRead)
    {
        if (entityNode.PropertyStates.TryGetValue(toRead.NodeId, out var propertyHandle))
            return propertyReader.ReadProperty(toRead, propertyHandle);
        if (entityNode.BaseObject.NodeId.Equals(toRead.NodeId)) return ReadEntity(toRead);

        return new EntityValueReadResponse(toRead, StatusCodes.BadNoEntryExists,
            $"The entity {entityNode.BaseObject.DisplayName} does not have any property with id {toRead.NodeId}");
    }

    private EntityValueReadResponse ReadEntity(ReadValueId readId)
    {
        var node = entityNode.BaseObject;
        return readId.AttributeId switch
        {
            Attributes.AccessLevel => CreateResponse(readId, AccessLevels.CurrentReadOrWrite),
            Attributes.DataType => CreateResponse(readId, DataTypes.ObjectNode),
            Attributes.BrowseName => CreateResponse(readId, node.BrowseName),
            Attributes.NodeClass => CreateResponse(readId, (int)NodeClass.Object),
            Attributes.DisplayName => CreateResponse(readId, node.DisplayName),
            Attributes.Description => CreateResponse(readId,
                new LocalizedText($"The managed entity '{node.DisplayName.ToString()}'")),
            Attributes.NodeId => CreateResponse(readId, node.NodeId),
            _ => new EntityValueReadResponse(readId, StatusCodes.BadNotSupported, "Not supported")
        };
    }


    private static (DataValue dataValue, StatusCode Good) AssignValue<T>(T value)
    {
        var dataValue = new DataValue();
        dataValue.StatusCode = StatusCodes.Good;
        dataValue.Value = value;
        return (dataValue, StatusCodes.Good);
    }

    private static EntityValueReadResponse CreateResponse<T>(ReadValueId readId, T valueGet)
    {
        return new EntityValueReadResponse(readId, () => AssignValue(valueGet));
    }
}