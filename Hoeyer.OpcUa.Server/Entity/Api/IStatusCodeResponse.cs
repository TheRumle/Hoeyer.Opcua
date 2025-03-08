using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api;

public interface IStatusCodeResponse : IRequestResponse
{
    public string StatusMessage => StatusCode.LookupSymbolicId(ResponseCode.Code);

    public StatusCode ResponseCode { get; }
}