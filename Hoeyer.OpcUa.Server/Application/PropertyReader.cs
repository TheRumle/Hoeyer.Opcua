using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal class PropertyReader : IPropertyReader
{
    public EntityValueReadResponse ReadProperty(ReadValueId readId, PropertyState node)
    {
        return readId.AttributeId switch
        {
            Attributes.BrowseName => CreateResponse(readId, node.BrowseName),
            Attributes.NodeClass => CreateResponse(readId, (int)NodeClass.Variable),
            Attributes.DisplayName => CreateResponse(readId, node.DisplayName),
            Attributes.Description => CreateResponse(readId, new LocalizedText(GetPropertyDescription(node))),
            Attributes.NodeId => CreateResponse(readId, node.NodeId),
            Attributes.Value => CreateResponse(readId, node.Value),
            Attributes.ValueRank => CreateResponse(readId, node.ValueRank),
            Attributes.MinimumSamplingInterval => CreateResponse(readId, node.MinimumSamplingInterval),
            Attributes.DataType => CreateResponse(readId, node.DataType),
            _ => CreateResponse(readId, StatusCodes.BadNotSupported)
        };
    }


    private static string GetPropertyDescription(PropertyState node)
    {
        var rank = node.ValueRank == ValueRanks.Scalar ? "(List)" : "";
        var type = node.WrappedValue.TypeInfo.BuiltInType;
        var typeDescr = $"{DataTypes.GetBrowseName((int)type)}{rank}";
        return $"The property {node.DisplayName.ToString()} of type '{typeDescr}'";
    }

    private static EntityValueReadResponse CreateResponse<T>(ReadValueId readId, T value)
    {
        return new EntityValueReadResponse(readId, () =>
        {
            var dataValue = new DataValue();
            dataValue.StatusCode = StatusCodes.Good;
            dataValue.Value = value;
            return (dataValue, StatusCodes.Good);
        });
    }
}