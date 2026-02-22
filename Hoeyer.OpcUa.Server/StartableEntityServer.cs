using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Services.Configuration;
using Microsoft.Extensions.Logging;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server;

internal sealed class StartableEntityServer(
    ILogger logger,
    ApplicationInstance applicationInstance,
    OpcEntityServer entityServer,
    IServerStartedHealthCheck healthCheckAssignment)
    : IStartableEntityServer, IStartedEntityServer
{
    private readonly ApplicationInstance _applicationInstance =
        applicationInstance ?? throw new ArgumentNullException(nameof(applicationInstance));

    private readonly OpcEntityServer _entityServer =
        entityServer ?? throw new ArgumentNullException(nameof(entityServer));

    private bool _disposed;

    public async Task<IStartedEntityServer> StartAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(StartableEntityServer));
        }

        if (healthCheckAssignment.IsServerStarted)
        {
            return this;
        }

        try
        {
            await _applicationInstance.Start(_entityServer);
            healthCheckAssignment.MarkCompleted();
        }
        catch (Exception e)
        {
            healthCheckAssignment.MarkFailed(e);
            logger.LogCritical(e, "Failed to start the server");
            throw new OpcUaEntityServerException("Failed to start the server", e);
        }

        return this;
    }

    /// <inheritdoc />
    public IOpcUaTargetServerSetup ServerInfo => _entityServer.ServerInfo;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _entityServer.Stop();
            _applicationInstance.Stop();
            _entityServer.Dispose();
        }

        _disposed = true;
    }

    ~StartableEntityServer()
    {
        Dispose(false);
    }
}