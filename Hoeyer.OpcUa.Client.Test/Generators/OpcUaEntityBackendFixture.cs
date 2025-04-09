using Hoeyer.OpcUa.Client.Application.MachineProxy;
using Hoeyer.OpcUa.Server.Core;
using Hoeyer.OpcUa.TestApplication;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.ClientTest.Generators;

public sealed record OpcUaEntityBackendFixture<TService> : IDisposable where TService : notnull
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly OpcUaEntityTestApplication _hostedApplication;
    private IServiceScope _scope;
    private readonly Type _implementationType;
    private bool _initialized = false;

    private async Task ServerStarted()
    {
        if (_initialized) return;
        await _hostedApplication.StartAsync(_cancellationTokenSource.Token);
        _scope = _hostedApplication.GetScope;
        await _scope.ServiceProvider.GetRequiredService<IStartableEntityServer>().StartAsync();
        var serverStarted = _scope.ServiceProvider.GetService<EntityServerStartedMarker>()!;
        await serverStarted;
        _initialized = true;
    }


    public OpcUaEntityBackendFixture(OpcUaEntityTestApplication hostedApplication, Type implementationType)
    {
        _hostedApplication = hostedApplication;
        _implementationType = implementationType;
    }

    public async Task<ISession> GetSession(string sessionid)
    {
        await ServerStarted();
        return await _scope.ServiceProvider.GetService<IEntitySessionFactory>()!.CreateSessionAsync(sessionid)!;
    }

    public async Task<TService> GetFixture()
    {
        await ServerStarted();
        return (TService)_scope.ServiceProvider.GetRequiredService(_implementationType);
    }

    /// <inheritdoc />
    void IDisposable.Dispose()
    {
        _hostedApplication.Dispose();
        _scope.Dispose();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}