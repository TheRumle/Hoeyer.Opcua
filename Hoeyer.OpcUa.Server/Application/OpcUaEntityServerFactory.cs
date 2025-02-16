using System.Collections.Generic;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.NodeManagement.Entity;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class OpcUaEntityServerFactory(
    EntityServerConfiguration entityServerConfiguration,
    IEnumerable<IEntityNodeCreator> entityObjectCreators,
    EntityNodeManagerFactory entityNodeManagerFactory)
{
    
    public StartableEntityServer CreateServer()
    {
        var configuration = ServerApplicationConfigurationFactory.CreateServerConfiguration(entityServerConfiguration);
        
        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = entityServerConfiguration.ServerName,
            ApplicationType = ApplicationType.Server
        };
        
        application.LoadApplicationConfiguration(false);

        var server = new OpcEntityServer(entityServerConfiguration, entityObjectCreators, entityNodeManagerFactory);
        return new StartableEntityServer(application, server);
    }
}