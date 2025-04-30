using System;
using System.Text.Json;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.RequestResponse;

public abstract class StatusCodeResponse<TRequest, TResponse>
    : IRequestResponse<TRequest, (TResponse DataValue, StatusCode StatusCode)>, IStatusCodeResponse
{
    protected StatusCodeResponse(TRequest request, StatusCode code, string? error = null)
    {
        Request = request;
        ResponseCode = code;
        if (error != null)
        {
            Error = error;
        }

        ResponseData = default!;
        Response = new ValueTuple<TResponse, StatusCode>(default!, code.Code);
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
            this.ResponseData = Response.DataValue;
        }
        catch (Exception e)
        {
            Response = default;
            IsSuccess = false;
            Error = e.Message;
        }
    }

    public TResponse ResponseData { get; }

    /// <inheritdoc />
    public string Error { get; } = "";


    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public TRequest Request { get; }

    /// <inheritdoc />
    public (TResponse DataValue, StatusCode StatusCode) Response { get; }

    /// <inheritdoc />
    public string StatusMessage => StatusCode.LookupSymbolicId(ResponseCode.Code);

    /// <inheritdoc />
    public StatusCode ResponseCode { get; }

    public override string ToString()
    {
        if (IsSuccess)
        {
            return SuccessDetails();
        }

        return ErrorDetails();
    }

    /// <summary>
    ///     Provides information about the <see cref="Request" />, the <see cref="Error" /> and the <see cref="ResponseCode" />
    ///     s.t a meaningful log can be created.
    /// </summary>
    /// <returns></returns>
    private string ErrorDetails()
    {
        return JsonSerializer.Serialize(new
        {
            Error,
            Request = RequestString(),
            ResponseCode = ResponseCode.ToString()
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    ///     Provide information about the <see cref="Request" /> and the <see cref="Response" /> to provide a meaningful
    ///     history log.
    /// </summary>
    /// <returns></returns>
    private string SuccessDetails()
    {
        return JsonSerializer.Serialize(new
        {
            Request = RequestString(),
            Response = ResponseCode.ToString()
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    public abstract string RequestString();
}