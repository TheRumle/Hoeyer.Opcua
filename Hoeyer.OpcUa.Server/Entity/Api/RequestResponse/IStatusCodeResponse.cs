using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;

public interface IStatusCodeResponse : IRequestResponse
{
    public string StatusMessage { get; }

    public StatusCode ResponseCode { get; }
}