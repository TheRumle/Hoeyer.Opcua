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
    OpcUaEntityServerConfigurationSetup opcUaEntityServerConfiguration,
    IEnumerable<IEntityNodeCreator> entityObjectCreators, 
    ILoggerFactory loggerFactory)
{
    
    public StartableEntityServer CreateServer()
    {
        var configuration = ServerApplicationConfigurationFactory.CreateServerConfiguration(opcUaEntityServerConfiguration);
        
        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = opcUaEntityServerConfiguration.ServerName,
            ApplicationType = ApplicationType.Server
        };
        
        application.LoadApplicationConfiguration(false);

        var entityNodeManagerFactory = new EntityNodeManagerFactory(loggerFactory);
        var entityServerLogger = loggerFactory.CreateLogger<OpcEntityServer>();
        
        var server = new OpcEntityServer(opcUaEntityServerConfiguration.EntityServerConfiguration, entityObjectCreators, entityNodeManagerFactory, entityServerLogger);
        return new StartableEntityServer(application, server);
    }
}