using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.RequestResponse;

public sealed record EntityValueReadResponse : StatusCodeResponse<ReadValueId, DataValue>
{
    public EntityValueReadResponse(ReadValueId request, StatusCode code, string? error = null)
        : base(request, code, error)
    {}

    public EntityValueReadResponse(ReadValueId request, Func<(DataValue, StatusCode)> valueGet) : base(request, valueGet)
    {}  
    
    public string AttributeName => Attributes.GetBrowseName(Request.AttributeId);
}