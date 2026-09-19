using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.Logging;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server;

internal sealed class StartableEntityServer(
    ILogger<StartableEntityServer> logger,
    ServerApplication applicationInstance,
    IOpcEntityServer entityServer,
    IServerStartedHealthCheck healthCheckAssignment)
    : IStartableEntityServer, IStartedEntityServer
{
    private readonly ApplicationInstance _applicationInstance =
        applicationInstance ?? throw new ArgumentNullException(nameof(applicationInstance));


    public async Task<IStartedEntityServer> StartAsync(CancellationToken token = default)
    {
        if (healthCheckAssignment.IsServerStarted)
        {
            return this;
        }

        try
        {
            await _applicationInstance.StartAsync(entityServer.AsServerBase());

            var nodesReadyTask = entityServer.DomainManager!
                .NodeManagers
                .OfType<IEntityNodeManager>()
                .Select(nodeManager => nodeManager.NodeReady)
                .ToArray();

            await Task.WhenAll(nodesReadyTask);
            healthCheckAssignment.MarkCompleted();
        }
        catch (Exception e)
        {
            healthCheckAssignment.MarkFailed(e);
            logger.LogCritical(e, "Failed to start the server: {}", e.Message);
            throw new OpcUaEntityServerException($"Failed to start the server due to error: {e.Message}", e);
        }

        return this;
    }

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