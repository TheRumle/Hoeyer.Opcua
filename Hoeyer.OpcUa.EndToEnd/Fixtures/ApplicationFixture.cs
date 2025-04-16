using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core.Reflections;
using Hoeyer.OpcUa.EndToEndTest.TestApplication;
using Hoeyer.OpcUa.Server;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

/// <summary>
/// A fixture with a hosted application with OpcUa server and clients. Has a method get any service from the service collection contained within. See <see cref="OpcUaClientAndServerFixture"/> for the concrete hosted application
/// </summary>
public sealed class ApplicationFixture : IAsyncDisposable, IAsyncInitializer
{
    private readonly OpcUaClientAndServerFixture _hostedApplication = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public CancellationToken Token => _cancellationTokenSource.Token;
    public IServiceScope Scope { get; private set; } = null!;
    private bool _initialized;
    
    public async Task<T> GetService<T>() where T : notnull
    {
        await InitializeAsync();
        return Scope.ServiceProvider.GetService<T>()!;
    }
    
    public async Task<T> GetService<T>(Type t) where T : notnull
    {
        await InitializeAsync();
        return (T)Scope.ServiceProvider.GetService(t)!;
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