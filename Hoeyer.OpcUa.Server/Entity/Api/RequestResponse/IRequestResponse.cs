namespace Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;

public interface IRequestResponse
{
    bool IsSuccess { get; }
    bool IsFailed => !IsSuccess;
}