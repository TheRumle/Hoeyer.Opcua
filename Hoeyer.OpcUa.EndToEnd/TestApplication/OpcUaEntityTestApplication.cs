using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Server;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.EndToEndTest.TestApplication;

public sealed class OpcUaEntityTestApplication : IHost
{
    private readonly IHost _hostedServer;
    public EntityServerStartedMarker Marker => _hostedServer.Services.GetService<EntityServerStartedMarker>()!;

    public T GetService<T>() => _hostedServer.Services.GetService<T>()!;
    public AsyncServiceScope GetAsyncScope => _hostedServer.Services.CreateAsyncScope();
    public IServiceScope GetScope => _hostedServer.Services.CreateScope();

    private readonly ReservedPort _reservedPort = new();
    public IEnumerable<ServiceDescriptor> ServiceDescriptors { get; private set;  }
        
    public OpcUaEntityTestApplication()
    {
        _hostedServer = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging => { logging.AddJsonConsole();
                logging.SetMinimumLevel(LogLevel.Error);
            })
            .ConfigureServices((context, services) =>
            {
    
                services.AddOpcUaServerConfiguration(conf => conf
                        .WithServerId("MyServer")
                        .WithServerName("My Server")
                        .WithHttpsHost("localhost", _reservedPort.Port)
                        .WithEndpoints([$"opc.tcp://localhost:{_reservedPort.Port}"])
                        .Build())
                    .WithEntityServices()
                    .WithOpcUaServer()
                    .WithOpcUaClientServices();

                ServiceDescriptors = services.Select(e=>e);
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