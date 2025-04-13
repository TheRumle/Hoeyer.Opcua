using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Server.Core;
using Hoeyer.OpcUa.TestApplication;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Client.EndToEnd.Generators;

public sealed class ApplicationFixture : IDisposable
{
    private readonly OpcUaEntityTestApplication _hostedApplication = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public IServiceScope Scope { get; private set; } = null!;
    private bool _initialized;
    public async Task<T?> GetService<T>() where T : notnull
    {
        await InitializeAsync();
        return Scope.ServiceProvider.GetRequiredService<T>();
    }

    public async Task<ISession> CreateSession(string sessionId)
    {
        await InitializeAsync();
        return await Scope.ServiceProvider.GetService<IEntitySessionFactory>()!.CreateSessionAsync(sessionId);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _hostedApplication.Dispose();
        _cancellationTokenSource.Dispose();
        Scope.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await CastAndDispose(_hostedApplication);
        await CastAndDispose(_cancellationTokenSource);
        await CastAndDispose(Scope);
        
        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
            {
                await resourceAsyncDisposable.DisposeAsync();
            }
            else
            {
                resource.Dispose();
            }
        }
    }

    /// <inheritdoc />
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


    /// <inheritdoc />
    public override string ToString()
    {
        var name = nameof(ApplicationFixture<int>);
        return $"{name}<{typeof(TService).Name}>";
    }

    public async Task<TService> GetFixture()
    {
        await _application.InitializeAsync();
        return (TService)_application.Scope.ServiceProvider.GetRequiredService(implementationType);
    }

    public Task<ISession> CreateSession(string sessionid) => _application.CreateSession(sessionid);
    public async Task<T?> GetService<T>() where T : notnull => await _application.GetService<T>();
}