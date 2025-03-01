using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.NodeManagement;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;



public sealed class OpcUaEntityServerFactory(
    OpcUaEntityServerSetup serverSetup,
    IEnumerable<IEntityNodeCreator> entityObjectCreators,
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
        
        var masterfactory = new DomainMasterNodeManagerFactory(new EntityNodeManagerFactory(loggerFactory), entityObjectCreators, serverSetup);
        var server = new OpcEntityServer(serverSetup, masterfactory, loggerFactory.CreateLogger<OpcEntityServer>());
        return new StartableEntityServer(application, server);
    }
}

internal sealed class DomainMasterNodeManagerFactory(EntityNodeManagerFactory entityManagerFactory,
    IEnumerable<IEntityNodeCreator> creators,
    IOpcUaEntityServerInfo info
    ) : IDomainMasterManagerFactory
{
    /// <inheritdoc />
    public DomainMasterNodeManager ConstructMasterManager(IServerInternal server, ApplicationConfiguration applicationConfiguration)
    {
        var additionalManagers = creators.Select(e => entityManagerFactory.Create(server, info.Host, e)).ToArray();
        return new DomainMasterNodeManager(server, applicationConfiguration, additionalManagers);
    }
}