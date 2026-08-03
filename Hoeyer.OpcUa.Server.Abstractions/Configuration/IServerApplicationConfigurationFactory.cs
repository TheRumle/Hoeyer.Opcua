using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Abstractions.Configuration;

public interface IServerApplicationConfigurationFactory
{
    ApplicationConfiguration CreateServerConfiguration();
}