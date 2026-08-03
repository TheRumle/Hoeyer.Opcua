namespace Hoeyer.OpcUa.Core.Configuration.Health;

public interface IServerStartedHealthCheck : IHealthCheckAssignment
{
    bool IsServerStarted { get; }
    Task<bool> ServerRunning();
    void MarkFailed(Exception exception);
}