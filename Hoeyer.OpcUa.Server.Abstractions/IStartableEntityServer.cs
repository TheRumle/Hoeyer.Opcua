namespace Hoeyer.OpcUa.Server.Abstractions;

public interface IStartableEntityServer
{
    Task<IStartedEntityServer> StartAsync(CancellationToken token = default);
}