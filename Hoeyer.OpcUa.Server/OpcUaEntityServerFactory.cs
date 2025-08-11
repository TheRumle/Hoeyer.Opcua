using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Services.Configuration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server;

internal sealed class OpcUaAgentServerFactory(
    AgentServerStartedMarker marker,
    OpcUaAgentServerSetup serverSetup,
    IEnumerable<IAgentManagerFactory> agentManagerFactories,
    ILoggerFactory loggerFactory) : IOpcUaAgentServerFactory
{
    private StartableAgentServer startable;

    public IStartableAgentServer CreateServer()
    {
        if (startable != null) return startable;
        ApplicationConfiguration configuration =
            ServerApplicationConfigurationFactory.CreateServerConfiguration(serverSetup);

        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = serverSetup.ApplicationName,
            ApplicationType = ApplicationType.Server
        };

        application.LoadApplicationConfiguration(false);

        var server = new OpcAgentServer(serverSetup, agentManagerFactories,
            loggerFactory.CreateLogger<OpcAgentServer>());
        startable = new StartableAgentServer(application, server, marker);
        return startable;
    }
}