using System;
using FluentResults;

namespace Hoeyer.OpcUa.Server.Application;

/// <summary>
/// An object that represents a Request and the Answer to that request. 
/// </summary>
/// <typeparam name="TRequest">The request to get the value represented in <typeparamref name="TResponse"/>. Will always be available.</typeparam>
/// <typeparam name="TResponse">The response to the request. If no response is produced then IsSuccess => false and error message will always be available.</typeparam>
public abstract class RequestResponse<TRequest, TResponse>
{
    public static implicit operator Result<(TRequest request, TResponse response)>(
        RequestResponse<TRequest, TResponse> value)
    {
        if (value.IsSuccess) return Result.Ok((value.Request, value.Response));
        return Result.Fail(value.Error);
    } 
    
    public Result<(TRequest request, TResponse response)> AsResult()
    {
        return this;
    } 
    
    /// <summary>
    /// The request object - this will always be available, even if no <see cref="RequestResponse{TRequest,TResponse}.Response"/> is available
    /// </summary>
    public TRequest Request { get; }
    
    /// <summary>
    /// The answer to the request - this will only be available if <see cref="RequestResponse{TRequest,TResponse}.IsSuccess"/> is true
    /// </summary>
    public TResponse Response { get; }
    public bool IsSuccess { get; }
    public bool IsFailed => !IsSuccess;
    /// <summary>
    /// A description of the error, if any error occurred.
    /// </summary>
    public string Error { get; } = "No error provided.";

    protected RequestResponse(TRequest request, Func<TResponse> response)
    {
        try
        {
            Request = request;
            Response = response.Invoke();
            IsSuccess = true;
        }
        catch (Exception e)
        {
            Response = default;
            IsSuccess = false;
            Error = e.Message;
        }
    }
    

    protected RequestResponse(TRequest request, TResponse response) : this(request, response, true)
    {}
    
    protected RequestResponse(TRequest request, string error) : this(request,default, false)
    {
        Error = error;
    }

    protected RequestResponse(TRequest request, TResponse response, bool isSuccess)
    {
        IsSuccess = isSuccess;
        Request = request;
        Response = response;
    }
}