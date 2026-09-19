using System.Net;
using System.Net.Sockets;
using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.IntegrationTest.Configuration;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Hoeyer.OpcUa.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter.LocallyHostedServer;

internal sealed class LocalHostedIntegrationTestEnvironment
    : IIntegrationTestEnvironment
{
    private IServerStartedHealthCheck _healthCheck = null!;

    private ServiceCollection _serviceCollection = new();
    private ServiceProvider _serviceProvider;
    public IServiceProvider AvailableServices => _serviceProvider;
    public OpcEnvironment OpcEnvironment { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var portListener = new TcpListener(IPAddress.Loopback, 0);
        portListener.Start();

        var port = ((IPEndPoint)portListener.LocalEndpoint).Port;

        var services = AddServices(port);
        _serviceProvider = services.BuildServiceProvider();
        _healthCheck = AvailableServices.GetRequiredService<IServerStartedHealthCheck>();
        var startableServer = AvailableServices.GetRequiredService<IStartableEntityServer>();

        await startableServer.StartAsync();
        await _healthCheck.ServerRunning();
    }

    public Task<bool> EnvironmentReady() => _healthCheck.ServerRunning();

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
    }

    private IServiceCollection AddServices(int port)
    {
        OpcEnvironment = OpcEnvironment.Default(port, "localhost");
        var testAssemblyMarker = typeof(TestEntity);

        _serviceCollection
            .AddSingleton<EnvironmentHealthCheck>(EnvironmentReady)
            .AddClientAndServerTestServices(OpcEnvironment, [testAssemblyMarker]);

        return _serviceCollection;
    }
}