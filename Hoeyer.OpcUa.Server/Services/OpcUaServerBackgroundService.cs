using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api;
using Microsoft.Extensions.Hosting;

namespace Hoeyer.OpcUa.Server.Services;

public sealed class OpcUaServerBackgroundService(IOpcUaEntityServerFactory factory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var server = factory.CreateServer();
        await server.StartAsync();
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}