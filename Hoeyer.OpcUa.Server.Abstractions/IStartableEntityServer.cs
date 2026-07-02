namespace Hoeyer.OpcUa.Server.Abstractions;

public interface IStartableEntityServer
{
    IOpcUaTargetServerSetup ServerInfo { get; }
    Task<IStartedEntityServer> StartAsync();
}