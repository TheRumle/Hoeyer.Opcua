using System.Collections.Generic;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.EntityNode;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class OpcUaEntityServerFactory(
    OpcUaEntityServerSetup opcUaEntityServer,
    IEnumerable<IEntityNodeCreator> entityObjectCreators, 
    ILoggerFactory loggerFactory)
{
    
    public IStartableEntityServer CreateServer()
    {
        var configuration = ServerApplicationConfigurationFactory.CreateServerConfiguration(opcUaEntityServer);
        
        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = opcUaEntityServer.ServerName,
            ApplicationType = ApplicationType.Server
        };
        
        application.LoadApplicationConfiguration(false);

        var entityNodeManagerFactory = new EntityNodeManagerFactory(loggerFactory);
        var entityServerLogger = loggerFactory.CreateLogger<OpcEntityServer>();
        
        var server = new OpcEntityServer(opcUaEntityServer, entityObjectCreators, entityNodeManagerFactory, entityServerLogger);
        return new StartableEntityServer(application, server);
    }
}