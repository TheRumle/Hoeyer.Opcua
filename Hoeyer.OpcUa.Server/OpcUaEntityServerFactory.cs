using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Services.Configuration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server;

internal sealed class OpcUaEntityServerFactory(
    EntityServerStartedMarker marker,
    OpcUaEntityServerSetup serverSetup,
    IDomainMasterManagerFactory masterNodeManagerFactory,
    ILoggerFactory loggerFactory) : IOpcUaEntityServerFactory
{
    private StartableEntityServer startable;

    public IStartableEntityServer CreateServer()
    {
        if (startable != null) return startable;
        ApplicationConfiguration configuration = ServerApplicationConfigurationFactory.CreateServerConfiguration(serverSetup);

        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = serverSetup.ApplicationName,
            ApplicationType = ApplicationType.Server
        };

        application.LoadApplicationConfiguration(false);

        var server = new OpcEntityServer(serverSetup, masterNodeManagerFactory, loggerFactory.CreateLogger<OpcEntityServer>());
        startable = new StartableEntityServer(application, server, marker);
        return startable;
    }
}