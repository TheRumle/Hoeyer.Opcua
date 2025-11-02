namespace Hoeyer.OpcUa.Core.Configuration.ServerTarget;

public interface IServerStartedHealthCheckMarker : IServerStartedHealthCheck
{
    public void MarkCompleted();
}