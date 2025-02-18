using System.Collections.Generic;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.Node.Entity;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class OpcUaEntityServerFactory(
    OpcUaEntityServerConfigurationSetup opcUaEntityServerConfiguration,
    IEnumerable<IEntityNodeCreator> entityObjectCreators,
    EntityNodeManagerFactory entityNodeManagerFactory)
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

        var server = new OpcEntityServer(opcUaEntityServerConfiguration.EntityServerConfiguration, entityObjectCreators, entityNodeManagerFactory);
        return new StartableEntityServer(application, server);
    }
}