using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hoeyer.OpcUa.Server.Services;

public sealed class OpcUaServerBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = serviceProvider.GetRequiredService<OpcUaAgentServerFactory>();
        var server = factory.CreateServer();
        await server.StartAsync();
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}