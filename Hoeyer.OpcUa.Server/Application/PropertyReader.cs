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
            Attributes.BrowseName => new EntityValueReadResponse(readId, () => AssignValue(node.BrowseName)),
            Attributes.NodeClass => new EntityValueReadResponse(readId, () => AssignValue((int)NodeClass.Variable)),
            Attributes.DisplayName => new EntityValueReadResponse(readId, () => AssignValue(node.DisplayName)),
            Attributes.Description => new EntityValueReadResponse(readId,
                () => AssignValue(new LocalizedText(GetPropertyDescription(node)))),
            Attributes.NodeId => new EntityValueReadResponse(readId, () => AssignValue(node.NodeId)),
            Attributes.Value => new EntityValueReadResponse(readId, () => AssignValue(node.Value)),
            Attributes.ValueRank => new EntityValueReadResponse(readId, () => AssignValue(node.ValueRank)),
            Attributes.MinimumSamplingInterval => new EntityValueReadResponse(readId,
                () => AssignValue(node.MinimumSamplingInterval)),
            Attributes.DataType => new EntityValueReadResponse(readId, () => AssignValue(node.DataType)),
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

    private static string GetPropertyDescription(PropertyState node)
    {
        var rank = node.ValueRank == ValueRanks.Scalar ? "(List)" : "";
        var type = node.WrappedValue.TypeInfo.BuiltInType;
        var typeDescr = $"{DataTypes.GetBrowseName((int)type)}{rank}";
        return $"The property {node.DisplayName.ToString()} of type '{typeDescr}'";
    }
}