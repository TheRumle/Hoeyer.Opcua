namespace Hoeyer.OpcUa.Core.Configuration.ServerTarget;

public interface IServerStartedHealthCheck : IHealthCheckAssignment
{
    bool IsServerStarted { get; }
    Task ServerRunning();
    void MarkFailed(Exception exception);
}