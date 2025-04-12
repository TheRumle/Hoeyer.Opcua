using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Server.Core;
using Hoeyer.OpcUa.TestApplication;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.ClientTest.Generators;

public sealed class ClientFixture<TService>(
    OpcUaEntityTestApplication hostedApplication,
    Type implementationType)
    : IDisposable
    where TService : notnull
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private IServiceScope Scope { get; set; } = null!;
    private bool _initialized = false;
    private Type EntityType => implementationType.GenericTypeArguments[0];

    private async Task ServerStarted()
    {
        if (_initialized) return;
        await hostedApplication.StartAsync(_cancellationTokenSource.Token);
        Scope = hostedApplication.GetScope;
        await Scope.ServiceProvider.GetRequiredService<IStartableEntityServer>().StartAsync();
        var serverStarted = Scope.ServiceProvider.GetService<EntityServerStartedMarker>()!;
        await serverStarted;
        _initialized = true;
    }
    
    public async Task<ISession> CreateSession(string sessionid)
    {
        await ServerStarted();
        return await Scope.ServiceProvider.GetService<IEntitySessionFactory>()!.CreateSessionAsync(sessionid)!;
    }


    /// <inheritdoc />
    public override string ToString()
    {
        var name = nameof(ClientFixture<int>);
        return $"{name}<{typeof(TService).Name}> ({EntityType.Name})";
    }


    public async Task<TService> GetFixture()
    {
        await ServerStarted();
        return (TService)Scope.ServiceProvider.GetRequiredService(implementationType);
    }
    
    public async Task<IClientServicesContainer> GetClientServices()
    {
        await ServerStarted();
        var containerType = typeof(ClientServicesContainer<>).MakeGenericType(EntityType);
        return (IClientServicesContainer)Scope.ServiceProvider.GetRequiredService(containerType);
    }
    
    public async Task<T?> GetService<T>() where T : notnull
    {
        await ServerStarted();
        return Scope.ServiceProvider.GetRequiredService<T>();
    }
    
    void IDisposable.Dispose()
    {
        hostedApplication.Dispose();
        Scope.Dispose();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }


}