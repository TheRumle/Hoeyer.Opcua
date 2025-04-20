using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.Server.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.EndToEndTest.TestApplication;

public sealed class OpcUaClientAndServerFixture : IHost
{
    private readonly IHost _hostedServer;
    public EntityServerStartedMarker Marker => _hostedServer.Services.GetService<EntityServerStartedMarker>()!;

    public T GetService<T>() => _hostedServer.Services.GetService<T>()!;
    public AsyncServiceScope GetAsyncScope => _hostedServer.Services.CreateAsyncScope();
    public IServiceScope GetScope => _hostedServer.Services.CreateScope();

        
    public OpcUaClientAndServerFixture()
    {
        _hostedServer = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging => { logging.AddJsonConsole();
                logging.SetMinimumLevel(LogLevel.Error);
            })
            .ConfigureServices((context, services) =>
            {
                foreach (var s in new AllOpcUaServicesFixture().ServiceCollection) services.Add(s);
            }).Build();
    }
    
    
    /// <inheritdoc />
    public void Dispose() => _hostedServer.Dispose();

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = new CancellationToken()) => _hostedServer.StartAsync(cancellationToken);

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken = new CancellationToken()) => _hostedServer.StopAsync(cancellationToken);

    /// <inheritdoc />
    public IServiceProvider Services => _hostedServer.Services;
}