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
    private ServiceProvider _serviceProvider = null!;
    private IServerStartedHealthCheck _healthCheck = null!;

    public IServiceProvider Services => _serviceProvider;
    public IServiceCollection AvailableServices { get; } = new ServiceCollection();
    public OpcEnvironment OpcEnvironment { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var portListener = new TcpListener(IPAddress.Loopback, 0);
        portListener.Start();

        try
        {
            var port = ((IPEndPoint)portListener.LocalEndpoint).Port;

            OpcEnvironment = OpcEnvironment.Default(port, "localhost");

            var testAssemblyMarker = typeof(TestEntity);

            AvailableServices.AddClientAndServerTestServices(
                OpcEnvironment,
                [testAssemblyMarker]);

            _serviceProvider = AvailableServices.BuildServiceProvider();

            var server = _serviceProvider.GetRequiredService<IStartableEntityServer>();
            _healthCheck = _serviceProvider.GetRequiredService<IServerStartedHealthCheck>();
            var startedServer = await server.StartAsync();
            await _healthCheck.ServerRunning();

            AvailableServices.AddSingleton(server);
            AvailableServices.AddSingleton(startedServer);
            AvailableServices.AddSingleton<EnvironmentHealthCheck>(EnvironmentReady);
        }
        finally
        {
            portListener.Dispose();
        }
    }

    public ValueTask DisposeAsync() =>
        _serviceProvider?.DisposeAsync() ?? ValueTask.CompletedTask;

    public Task<bool> EnvironmentReady() =>
        _healthCheck.ServerRunning();
}