using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.Configuration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server;

internal sealed class OpcUaEntityServerFactory(
    ILogger<OpcUaEntityServerFactory> entityServerFactoryLogger,
    IServerApplicationConfigurationFactory applicationConfigurationFactory,
    IServerStartedHealthCheck assignment,
    IOpcUaTargetServerSetup serverSetup,
    IOpcEntityServer server,
    ILoggerFactory loggerFactory) : IOpcUaEntityServerFactory
{
    private StartableEntityServer? _startable;

    public IStartableEntityServer CreateServer()
    {
        using var scope = entityServerFactoryLogger.BeginScope("CreateServerAsync");
        if (_startable != null)
        {
            return _startable;
        }

        var configuration = applicationConfigurationFactory.CreateServerConfiguration();

        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = serverSetup.ApplicationName,
            ApplicationType = ApplicationType.Server
        };

        var logger = loggerFactory.CreateLogger<StartableEntityServer>();
        _startable = new StartableEntityServer(logger, application, server, assignment);
        return _startable;
    }
}