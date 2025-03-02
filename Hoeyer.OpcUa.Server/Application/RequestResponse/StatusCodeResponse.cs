using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.RequestResponse;

public abstract record StatusCodeResponse<TRequest, TResponse> 
    : IRequestResponse<TRequest, (TResponse DataValue, StatusCode StatusCode)>, IStatusCodeResponse
{
    protected StatusCodeResponse(TRequest request, StatusCode code, string? error = null)
    {
        Request = request;
        ResponseCode = code;
        if (error != null) Error = error;
    }

    protected StatusCodeResponse(TRequest request, Func<(TResponse, StatusCode)> response)
    {
        Request = request;
        try
        {
            Request = request;
            Response = response.Invoke();
            ResponseCode = Response.StatusCode;
            IsSuccess = true;
        }
        catch (Exception e)
        {
            Response = default;
            IsSuccess = false;
            Error = e.Message;
        }
    } 
    

    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public string Error { get; } = "";

    /// <inheritdoc />
    public TRequest Request { get; }

    /// <inheritdoc />
    public (TResponse DataValue, StatusCode StatusCode) Response { get; }

    /// <inheritdoc />
    public StatusCode ResponseCode { get; }
}