namespace Hoeyer.OpcUa.Server.Abstractions.Configuration;

public interface IServerApplicationConfigurationFactory
{
    ServerApplicationConfiguration CreateServerConfiguration();
}