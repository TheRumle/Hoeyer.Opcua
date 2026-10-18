using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.Configuration;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Services;

internal sealed class ServerApplication : ApplicationInstance
{
    public ServerApplication(
        ITelemetryContext context,
        IOpcUaTargetServerSetup setup,
        IServerApplicationConfigurationFactory factory): base(context)
    {
        ApplicationConfiguration = factory.CreateServerConfiguration().OpcUaApplicationConfiguration;
        ApplicationName = setup.ApplicationName;
        ApplicationType = ApplicationType.Server;
    }
}