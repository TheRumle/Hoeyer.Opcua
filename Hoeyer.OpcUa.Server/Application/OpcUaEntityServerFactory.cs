using System.Collections.Generic;
using Hoeyer.OpcUa.Nodes;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class OpcUaEntityServerFactory(
    EntityServerConfiguration entityServerConfiguration,
    IEnumerable<IEntityNodeCreator> entityObjectCreators)
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

        var server = new OpcEntityServer(new EntityMasterNodeManagerFactory(entityObjectCreators), entityServerConfiguration);
        return new StartableEntityServer(application, server);
    }
}