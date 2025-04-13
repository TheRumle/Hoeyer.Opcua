using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Server.Core;
using Hoeyer.OpcUa.TestApplication;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.EndToEnd.Generators;

public sealed class ApplicationFixture : IDisposable
{
    private readonly OpcUaEntityTestApplication _hostedApplication = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public IServiceScope Scope { get; private set; } = null!;
    private bool _initialized;


    internal async Task ServerStarted()
    {
        if (_initialized) return;
        await _hostedApplication.StartAsync(_cancellationTokenSource.Token);
        Scope = _hostedApplication.GetScope;
        await Scope.ServiceProvider.GetRequiredService<IStartableEntityServer>().StartAsync();
        var serverStarted = Scope.ServiceProvider.GetService<EntityServerStartedMarker>()!;
        await serverStarted;
        _initialized = true;
    }
    
    public async Task<T?> GetService<T>() where T : notnull
    {
        await ServerStarted();
        return Scope.ServiceProvider.GetRequiredService<T>();
    }
    
    public async Task<ISession> CreateSession(string sessionid)
    {
        await ServerStarted();
        return await Scope.ServiceProvider.GetService<IEntitySessionFactory>()!.CreateSessionAsync(sessionid)!;
    }


    /// <inheritdoc />
    public void Dispose()
    {
        _hostedApplication.Dispose();
        _cancellationTokenSource.Dispose();
        Scope.Dispose();
    }
}

/// <summary>
/// A fixture with a hosted application with OpcUa server and clients. Has a method get the <typeparamref name="TService"></typeparamref>
/// </summary>
/// <param name="implementationType">The type of the concrete implementation used for <typeparamref name="TService"/></param>
/// <typeparam name="TService">The service that is guaranteed to be there</typeparam>
public sealed class ApplicationFixture<TService>(Type implementationType)
    where TService : notnull
{
    private readonly ApplicationFixture _application = new();
    private Type EntityType => implementationType.GenericTypeArguments[0];


    /// <inheritdoc />
    public override string ToString()
    {
        var name = nameof(ApplicationFixture<int>);
        return $"{name}<{typeof(TService).Name}> ({EntityType.Name})";
    }

    public async Task<TService> GetFixture()
    {
        await _application.ServerStarted();
        return (TService)_application.Scope.ServiceProvider.GetRequiredService(implementationType);
    }
    
    [SuppressMessage("Maintainability", "S1944", Justification = "The container services are wired up by the test application project")]
    public async Task<IClientServicesContainer> GetClientServices()
    {
        await _application.ServerStarted();
        var containerType = typeof(ClientServicesContainer<>).MakeGenericType(EntityType);
        return (IClientServicesContainer)_application.Scope.ServiceProvider.GetRequiredService(containerType);
    }
    
    public Task<ISession> CreateSession(string sessionid) => _application.CreateSession(sessionid);
}