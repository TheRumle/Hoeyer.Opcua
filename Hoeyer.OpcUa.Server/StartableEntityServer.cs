using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Server.Abstractions;
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


    public async Task<IStartedEntityServer> StartAsync()
    {
        if (healthCheckAssignment.IsServerStarted)
        {
            return this;
        }

        try
        {
            await _applicationInstance.StartAsync(entityServer);
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
    public IOpcUaTargetServerSetup ServerInfo => entityServer.ServerInfo;

    public async ValueTask DisposeAsync()
    {
        await CastAndDispose(entityServer);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
            {
                await resourceAsyncDisposable.DisposeAsync();
            }
            else
            {
                resource.Dispose();
            }
        }
    }
}