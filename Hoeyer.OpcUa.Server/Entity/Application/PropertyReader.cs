using System.Collections;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Opc.Ua;
using LocalizedText = Opc.Ua.LocalizedText;

namespace Hoeyer.OpcUa.Server.Entity.Application;

internal class PropertyReader(PermissionType permissionType) : IPropertyReader
{
    public PropertyReader():this(PermissionType.Browse | PermissionType.Read | PermissionType.Write | PermissionType.ReadRolePermissions | PermissionType.Call | PermissionType.ReceiveEvents)
    {}
    
    
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
            Attributes.ArrayDimensions => CreateResponse(readId,
                node.Value is ICollection c 
                    ? new ReadOnlyList<uint>(new List<uint> {(uint)c.Count})
                    : new ReadOnlyList<uint>(new List<uint> {})),
            Attributes.AccessLevel => CreateResponse(readId, AccessLevels.CurrentReadOrWrite),
            Attributes.UserAccessLevel => CreateResponse(readId, AccessLevels.CurrentReadOrWrite),
            Attributes.Historizing => CreateResponse(readId, false),
            Attributes.RolePermissions => CreateResponse(readId, new []{new RolePermissionType()
            {
                RoleId = ObjectIds.WellKnownRole_Anonymous,
                Permissions = (uint) permissionType
            }}),
            Attributes.UserRolePermissions => CreateResponse(readId, new []{new RolePermissionType()
            {
                RoleId = ObjectIds.WellKnownRole_Anonymous,
                Permissions = (uint) permissionType
            }}),
            Attributes.AccessRestrictions => CreateResponse(readId, (ushort)0x80),
            _ => CreateResponse(readId, StatusCodes.BadNotSupported)
        };
    }


    private static string GetPropertyDescription(PropertyState node)
    {
        var rank = node.ValueRank == ValueRanks.Scalar ? "" : "(List)";
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