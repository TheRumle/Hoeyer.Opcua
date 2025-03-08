using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;

public sealed record EntityValueReadResponse : StatusCodeResponse<ReadValueId, DataValue>
{

    public EntityValueReadResponse(ReadValueId request, StatusCode code, string error)
        : base(request, code, error)
    {
    }

    public EntityValueReadResponse(ReadValueId request, Func<(DataValue, StatusCode)> valueGet) : base(request,
        valueGet)
    {
    }

    public string AttributeName => Attributes.GetBrowseName(Request.AttributeId);

    /// <inheritdoc />
    public override string ToString()
    {
        if (IsSuccess) return AttributeName;
        return $"{AttributeName} : {Error}";
    }
}