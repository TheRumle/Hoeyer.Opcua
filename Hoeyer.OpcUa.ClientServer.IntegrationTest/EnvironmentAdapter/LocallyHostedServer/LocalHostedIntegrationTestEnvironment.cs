using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Hoeyer.OpcUa.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter.LocallyHostedServer;

internal sealed class LocalHostedIntegrationTestEnvironment(ClientAndServerIntegrationServices services)
    : IIntegrationTestEnvironment
{
    private IStartableEntityServer _server = null!;
    private IServerStartedHealthCheck _healthCheck = null!;

    public async Task InitializeAsync()
    {
        _server = services.Scope.ServiceProvider.GetRequiredService<IStartableEntityServer>();
        _healthCheck = services.Scope.ServiceProvider.GetRequiredService<IServerStartedHealthCheck>();
        await _server.StartAsync();
        await _healthCheck.ServerRunning();
    }

    public ValueTask DisposeAsync() => default;

    public int SimulationPort { get; } = services.BaseConfiguration.Port;
    public string Host { get; } = services.BaseConfiguration.HostName;
    public WebProtocol Protocol { get; } = services.BaseConfiguration.Protocol;
    public string ServerId { get; } = services.BaseConfiguration.OpcUaServerId;
    public string ServerName { get; } = services.BaseConfiguration.OpcUaServerName;
    public Task<bool> EnvironmentReady() => _healthCheck.ServerRunning();
}