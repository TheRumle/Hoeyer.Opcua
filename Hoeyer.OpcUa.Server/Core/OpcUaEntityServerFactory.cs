using Hoeyer.OpcUa.Server.Configuration;
using Hoeyer.OpcUa.Server.Entity.Management;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Core;

public sealed class OpcUaEntityServerFactory(
    OpcUaEntityServerSetup serverSetup,
    IDomainMasterManagerFactory masterNodeManagerFactory,
    ILoggerFactory loggerFactory)
{
    public IStartableEntityServer CreateServer()
    {
        var configuration = ServerApplicationConfigurationFactory.CreateServerConfiguration(serverSetup);

        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = serverSetup.ApplicationName,
            ApplicationType = ApplicationType.Server
        };

        application.LoadApplicationConfiguration(false);

        var server = new OpcEntityServer(serverSetup, masterNodeManagerFactory,
            loggerFactory.CreateLogger<OpcEntityServer>());
        return new StartableEntityServer(application, server);
    }
}