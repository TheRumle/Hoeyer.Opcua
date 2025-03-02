namespace Hoeyer.OpcUa.Server.Application.RequestResponse;

public interface IRequestResponse
{
    bool IsSuccess { get; }
    bool IsFailed => !IsSuccess;

    /// <summary>
    ///     A description of the error, if any error occurred.
    /// </summary>
    string Error { get; }
}