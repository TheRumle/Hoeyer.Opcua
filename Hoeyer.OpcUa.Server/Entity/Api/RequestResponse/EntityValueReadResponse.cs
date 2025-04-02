using System;
using Hoeyer.OpcUa.Core.Extensions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;

public sealed class EntityValueReadResponse : StatusCodeResponse<ReadValueId, DataValue>
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
    protected override string RequestString()
    {
        return $"Read {Request.AttributeId.AttributeName()}";
    }
}