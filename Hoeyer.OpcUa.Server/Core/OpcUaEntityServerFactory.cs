using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Entity.Management;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Core;

public sealed class OpcUaEntityServerFactory(
    OpcUaEntityServerSetup serverSetup,
    IEnumerable<IEntityNodeFactory> entityObjectCreators,
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

        var masterfactory = new DomainMasterNodeManagerFactory(
            new EntityNodeManagerFactory(loggerFactory),
            entityObjectCreators,
            serverSetup);
        var server = new OpcEntityServer(serverSetup, masterfactory, loggerFactory.CreateLogger<OpcEntityServer>());
        return new StartableEntityServer(application, server);
    }
}