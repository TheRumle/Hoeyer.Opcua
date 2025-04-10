namespace Hoeyer.OpcUa.Core.Application.RequestResponse;

public interface IRequestResponse<out TRequest, out TResponse> : IRequestResponse
{
    /// <summary>
    ///     The request object - this will always be available, even if no
    ///     <see cref="IRequestResponse{TRequest,TResponse}.Response" /> is available
    /// </summary>
    public TRequest Request { get; }

    /// <summary>
    ///     The answer to the request - this will only be available if
    ///     <see cref="IRequestResponse{TRequest,TResponse}.IsSuccess" /> is true
    /// </summary>
    public TResponse Response { get; }
}

public interface IRequestResponse
{
    bool IsSuccess { get; }
}