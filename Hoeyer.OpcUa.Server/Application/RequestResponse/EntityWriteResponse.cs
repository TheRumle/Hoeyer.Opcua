using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.RequestResponse;

public sealed record EntityWriteResponse : StatusCodeResponse<WriteValue, ServiceResult>
{
    public EntityWriteResponse(WriteValue request, ServiceResult result, string error) : base(request,
        result.StatusCode, error)
    {
    }

    public EntityWriteResponse(WriteValue request, ServiceResult valueGet) : base(request, valueGet.StatusCode)
    {
    }

    public string AttributeName => Attributes.GetBrowseName(Request.AttributeId);
}