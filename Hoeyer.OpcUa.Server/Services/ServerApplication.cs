using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.Configuration;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Services;

internal sealed class ServerApplication : ApplicationInstance
{
    public ServerApplication(
        IOpcUaTargetServerSetup setup,
        IServerApplicationConfigurationFactory factory)
    {
        ApplicationConfiguration = factory.CreateServerConfiguration();
        ApplicationName = setup.ApplicationName;
        ApplicationType = ApplicationType.Server;
    }
}