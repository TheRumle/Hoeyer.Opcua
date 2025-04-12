using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.RequestResponse;

public interface IStatusCodeResponse : IRequestResponse
{
    public string StatusMessage { get; }

    public StatusCode ResponseCode { get; }
}