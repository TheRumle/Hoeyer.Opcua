using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Configuration;
using Hoeyer.OpcUa.Server.Entity.Management;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server;

public sealed class OpcUaEntityServerFactory(
    EntityServerStartedMarker marker,
    OpcUaEntityServerSetup serverSetup,
    IDomainMasterManagerFactory masterNodeManagerFactory,
    ILoggerFactory loggerFactory)
{
    public IStartableEntityServer CreateServer()
    {
        ApplicationConfiguration configuration = ServerApplicationConfigurationFactory.CreateServerConfiguration(serverSetup);

        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = serverSetup.ApplicationName,
            ApplicationType = ApplicationType.Server
        };

        application.LoadApplicationConfiguration(false);

        var server = new OpcEntityServer(marker, serverSetup, masterNodeManagerFactory,
            loggerFactory.CreateLogger<OpcEntityServer>());
        return new StartableEntityServer(application, server);
    }
}