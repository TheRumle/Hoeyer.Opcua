using Hoeyer.OpcUa.Client.Application.MachineProxy;
using Hoeyer.OpcUa.Server.Core;
using Hoeyer.OpcUa.TestApplication;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.ClientTest.Generators;

public sealed record OpcClientServiceFixture<TService> : IDisposable where TService : notnull
{
    private Type EntityType => ImplementationType.GenericTypeArguments[0];
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly OpcUaEntityTestApplication _hostedApplication;
    private IServiceScope _scope;
    public readonly Type ImplementationType;
    private bool _initialized = false;

    /// <inheritdoc />
    public override string ToString()
    {
        var name = nameof(OpcClientServiceFixture<int>);
        return $"{name}<{typeof(TService).Name}> ({EntityType.Name})";
    }

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

    public OpcClientServiceFixture(OpcUaEntityTestApplication hostedApplication, Type implementationType)
    {
        _hostedApplication = hostedApplication;
        ImplementationType = implementationType;
    }

    public async Task<ISession> GetSession(string sessionid)
    {
        await ServerStarted();
        return await _scope.ServiceProvider.GetService<IEntitySessionFactory>()!.CreateSessionAsync(sessionid)!;
    }

        public async Task<TService> GetFixture()
        {
            await ServerStarted();
            return (TService)_scope.ServiceProvider.GetRequiredService(ImplementationType);
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