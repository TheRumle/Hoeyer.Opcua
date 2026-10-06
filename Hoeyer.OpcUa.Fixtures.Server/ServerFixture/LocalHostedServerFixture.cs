using System.Net;
using System.Net.Sockets;
using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.Fixtures.Common.Utils;
using Hoeyer.OpcUa.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Fixtures.Server.ServerFixture;

public sealed class LocalHostedServerFixture(Action<IServiceCollection, OpcEnvironment> configureServices)
    : IServerFixture
{
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public IServiceProvider AvailableServices => ConfiguredServices.ServiceProvider;
    private TestServiceCollection ConfiguredServices { get; set; } = null!;
    
    public async Task InitializeAsync()
    {
        await _initializationLock.WaitAsync();
        try
        {
            if (_initialized)
            {
                return;
            }

            var portListener = new TcpListener(IPAddress.Loopback, 0);
            portListener.Start();

            var port = ((IPEndPoint)portListener.LocalEndpoint).Port;

            var env = OpcEnvironment.Default(port, "localhost");
            ConfiguredServices = new TestServiceCollection(services => configureServices(services, env));
            
            var healthCheck = ConfiguredServices.ServiceProvider.GetRequiredService<IServerStartedHealthCheck>();
            var startableServer = ConfiguredServices.ServiceProvider.GetRequiredService<IStartableEntityServer>();

            await startableServer.StartAsync();
            await healthCheck.ServerRunning();
            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();  
        }
    }

    public async ValueTask DisposeAsync() => _initializationLock.Dispose();
}