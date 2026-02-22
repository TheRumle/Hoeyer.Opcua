using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Services.Configuration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server;

internal sealed class OpcUaEntityServerFactory(
    IServerStartedHealthCheck assignment,
    IOpcUaTargetServerSetup serverSetup,
    IEnumerable<IEntityNodeManagerFactory> entityManagerFactories,
    ILoggerFactory loggerFactory) : IOpcUaEntityServerFactory
{
    private StartableEntityServer? _startable;

    public IStartableEntityServer CreateServer()
    {
        if (_startable != null)
        {
            return _startable;
        }

        var configuration = ServerApplicationConfigurationFactory.CreateServerConfiguration(serverSetup);

        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = serverSetup.ApplicationName,
            ApplicationType = ApplicationType.Server
        };

        application.LoadApplicationConfiguration(false);

        var logger = loggerFactory.CreateLogger<OpcEntityServer>();
        var server = new OpcEntityServer(serverSetup, entityManagerFactories, logger);
        _startable = new StartableEntityServer(logger, application, server, assignment);
        return _startable;
    }
}