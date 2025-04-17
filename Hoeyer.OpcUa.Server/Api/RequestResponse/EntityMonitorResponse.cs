using Hoeyer.OpcUa.Core.Application.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api.RequestResponse;

public sealed record EntityMonitorResponse(bool IsSuccess, string StatusMessage, StatusCode ResponseCode) : IStatusCodeResponse
{
    public StatusCode ResponseCode { get; } = ResponseCode;
    public bool IsSuccess { get; } = IsSuccess;
    public string StatusMessage { get; } = StatusMessage;
}