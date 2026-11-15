namespace Hoeyer.OpcUa.Client.Abstractions.Configuration;

public interface IClientApplicationConfigurationFactory
{
    ClientApplicationConfiguration CreateClientConfiguration();
}