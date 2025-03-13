using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hoeyer.OpcUa.Server;

public sealed class OpcUaServerBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public OpcUaServerBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        OpcUaEntityServerFactory factory = _serviceProvider.GetRequiredService<OpcUaEntityServerFactory>();
        IStartableEntityServer server = factory.CreateServer();
        await server.StartAsync();
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}