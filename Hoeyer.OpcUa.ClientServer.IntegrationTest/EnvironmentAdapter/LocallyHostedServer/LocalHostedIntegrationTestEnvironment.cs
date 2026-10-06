using System.Net;
using System.Net.Sockets;
using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.Fixtures.Common.Utils;
using Hoeyer.OpcUa.IntegrationTest.Configuration;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Hoeyer.OpcUa.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter.LocallyHostedServer;

internal sealed class LocalHostedIntegrationTestEnvironment
    : IIntegrationTestEnvironment
{
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    private IServerStartedHealthCheck _healthCheck = null!;

    public TestServiceCollection AvailableServices { get; private set; }
    public OpcEnvironment OpcEnvironment { get; private set; } = null!;

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

            AvailableServices = new TestServiceCollection(services => ConfigureServices(services, port));
            
            _healthCheck = AvailableServices.ServiceProvider.GetRequiredService<IServerStartedHealthCheck>();
            var startableServer = AvailableServices.ServiceProvider.GetRequiredService<IStartableEntityServer>();

            await startableServer.StartAsync();
            await _healthCheck.ServerRunning();
            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public Task<bool> EnvironmentReady() => _healthCheck.ServerRunning();

    private void ConfigureServices(IServiceCollection collection, int port)
    {
        OpcEnvironment = OpcEnvironment.Default(port, "localhost");
        var testAssemblyMarker = typeof(TestEntity);

        collection
            .AddSingleton<EnvironmentHealthCheck>(EnvironmentReady)
            .AddClientAndServerTestServices(OpcEnvironment, [testAssemblyMarker]);
    }
}