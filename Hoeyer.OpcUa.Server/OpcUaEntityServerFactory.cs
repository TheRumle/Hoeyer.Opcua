using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Services.Configuration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server;

internal sealed class OpcUaEntityServerFactory(
    EntityServerStartedMarker marker,
    OpcUaTargetServerSetup serverSetup,
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

        var server = new OpcEntityServer(serverSetup, entityManagerFactories,
            loggerFactory.CreateLogger<OpcEntityServer>());
        _startable = new StartableEntityServer(application, server, marker);
        return _startable;
    }
}