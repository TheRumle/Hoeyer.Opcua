using Hoeyer.OpcUa.Server.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Hoeyer.OpcUa.Server.Services;

public sealed class OpcUaServerBackgroundService(IStartableEntityServer server) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await server.StartAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}