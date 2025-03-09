using Hoeyer.OpcUa.Core.Extensions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;

public sealed class EntityWriteResponse : StatusCodeResponse<WriteValue, ServiceResult>
{
    private EntityWriteResponse(WriteValue request, ServiceResult result, string error) : base(request,
        result.StatusCode, error)
    {
    }

    public EntityWriteResponse(WriteValue request, ServiceResult valueGet) : base(request, valueGet.StatusCode)
    {
    }

    public static EntityWriteResponse AttributeNotSupported(WriteValue request)
    {
        return new EntityWriteResponse(request, StatusCodes.BadNotSupported, "The current implementation does not support the reading the attribute.");
    }
    
    public static EntityWriteResponse AssignmentFailure(WriteValue request, NodeState state)
    {
        return new EntityWriteResponse(request, StatusCodes.BadNotSupported, $"Assignment to {state.BrowseName.Name} failed");
    }
    
    public static EntityWriteResponse NoMatch(WriteValue request)
    {
        return new EntityWriteResponse(request, StatusCodes.BadNoMatch,
            $"The entity does not have any data with NodeId '{request.NodeId}'!");
    }

    public string AttributeName => Attributes.GetBrowseName(Request.AttributeId);

    /// <inheritdoc />
    protected override string RequestString()
    {
        return $"Assign '{Request.AttributeId.AttributeName()}' to '{Request.Value.Value}'";
    }
}