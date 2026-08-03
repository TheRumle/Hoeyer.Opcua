using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Abstractions.Configuration;

public interface IClientApplicationConfigurationFactory
{
    ApplicationConfiguration CreateClientConfiguration();
}