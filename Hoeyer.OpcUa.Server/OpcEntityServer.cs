using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Core.Extensions.Logging;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;

internal sealed class OpcEntityServer(
    ITelemetryContext telemetryContext,
    IOpcUaTargetServerSetup applicationProductDetails,
    IEnumerable<IEntityNodeManagerFactory> entityManagerFactories,
    ILogger<OpcEntityServer> logger,
    IServerStartedHealthCheck healthCheck)
    : StandardServer(telemetryContext), IOpcEntityServer
{
    private static readonly DateTime BuildDate = DateTime.UtcNow;
    public ISystemContext DefaultContext => ServerInternal.DefaultSystemContext;
    public readonly IOpcUaTargetServerSetup ServerInfo = applicationProductDetails;

    private bool _disposed;

    public IDomainMasterNodeManager? DomainManager { get; private set; }
    public ServerBase AsServerBase() => this;

    public IEnumerable<IEntityManagerHolder> Managers => entityManagerFactories.OfType<IEntityManagerHolder>();
    
    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server,
        ApplicationConfiguration configuration)
    {
        return logger.Try(() =>
        {
            var managers = entityManagerFactories
                .Select(factory => factory.CreateEntityManager(server))
                .ToArray();

            var manager = new DomainMasterNodeManager(server, configuration, managers);
            DomainManager = manager;
            return manager;
        })!;
    }

    protected override async ValueTask StartApplicationAsync(ApplicationConfiguration configuration,
        CancellationToken cancellationToken = new())
    {
        try
        {
            logger.LogInformation("Starting application with configuration {@Configuration}",
                configuration.ToLoggingObject());
            await base.StartApplicationAsync(configuration, cancellationToken);
        }
        catch (ServiceResultException e) when (e.InnerException?.Message.Contains($"[{nameof(ManagerAlreadyInitializedException)}]") ?? false)
        {
            logger.LogCritical(e, "The entity manager has been initialized more than once.");
            throw e.InnerException;
        }
        catch (Exception e)
        {
            throw new OpcUaEntityServerException("A critical error occured when trying to start the OpcEntityServer.", e);
        }
    }

    protected override ServerProperties LoadServerProperties()
    {
        var properties = base.LoadServerProperties();
        properties.ProductName = ServerInfo.ApplicationName;
        properties.ProductUri = ServerInfo.ApplicationNamespace.ToString();
        properties.SoftwareVersion = "0.0.01";
        properties.BuildDate = BuildDate;
        return properties;
    }


    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (!disposing)
        {
            return;
        }

        logger.LogInformation("Closing server...");

        if (healthCheck.IsServerStarted)
        {
            DomainManager?.Dispose();
        }
        else
        {
            logger.LogWarning(
                "The server never reached a valid started state; skipping disposal of the domain manager to avoid tearing down partially-initialized internals.");
        }

        base.Dispose(disposing);
        _disposed = true;
    }
}