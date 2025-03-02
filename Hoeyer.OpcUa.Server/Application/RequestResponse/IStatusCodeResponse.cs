using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.RequestResponse;

public interface IStatusCodeResponse : IRequestResponse
{
    public string StatusMessage => StatusCode.LookupSymbolicId(ResponseCode.Code);
    public StatusCode ResponseCode
    {
        get;
    }
}