using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core.Reflections;
using Hoeyer.OpcUa.Server.Core;
using Hoeyer.OpcUa.TestApplication;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Client.EndToEnd.Generators;

public sealed class ApplicationFixture : IAsyncDisposable, IAsyncInitializer
{
    private readonly OpcUaEntityTestApplication _hostedApplication = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public IServiceScope Scope { get; private set; } = null!;
    private bool _initialized;
    public T? GetService<T>() where T : notnull
    {
        return Scope.ServiceProvider.GetService<T>();
    }

    public async Task<ISession> CreateSession(string sessionId)
    {
        return await Scope.ServiceProvider.GetService<IEntitySessionFactory>()!.CreateSessionAsync(sessionId);
    }
    
    public async Task InitializeAsync()
    {
        if (_initialized) return;
        await _hostedApplication.StartAsync(_cancellationTokenSource.Token);
        Scope = _hostedApplication.GetScope;
        await Scope.ServiceProvider.GetRequiredService<IStartableEntityServer>().StartAsync();
        var serverStarted = Scope.ServiceProvider.GetService<EntityServerStartedMarker>()!;
        await serverStarted;
        _initialized = true;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Dispose();
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// A fixture with a hosted application with OpcUa server and clients. Has a method get the <typeparamref name="TService"></typeparamref>
/// </summary>
/// <param name="typeContext">The type of the concrete implementation used for <typeparamref name="TService"/></param>
/// <typeparam name="TService">The service that is guaranteed to be there</typeparam>
public sealed class ApplicationFixture<TService>(EntityServiceTypeContext typeContext) : IAsyncDisposable, IAsyncInitializer
    where TService : notnull
{
    private readonly ApplicationFixture _application = new();

    /// <inheritdoc />
    public override string ToString()
    {
        return typeContext.Entity.Name;
    }

    public async Task InitializeAsync()
    {
        await _application.InitializeAsync();
    }

    public async Task<TService> GetFixture()
    {
        await _application.InitializeAsync();
        return (TService)_application
            .Scope
            .ServiceProvider
            .GetRequiredService(typeContext.ConcreteServiceType);
    }

    public Task<ISession> CreateSession(string sessionid) => _application.CreateSession(sessionid);
    
    public T? GetService<T>() where T : notnull => _application.GetService<T>();
    
    public async ValueTask DisposeAsync()
    {
        await _application.DisposeAsync();
    }
}