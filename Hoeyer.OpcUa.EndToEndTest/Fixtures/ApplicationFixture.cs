using Hoeyer.Common.Extensions.Types;
using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class ApplicationFixture(WebProtocol protocol) : IAsyncDisposable, IAsyncInitializer
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _isInitialized;
    private ReservedPort _reservedPort = null!;

    public ApplicationFixture() : this(WebProtocol.OpcTcp)
    {
    }

    public CancellationToken Token => _cancellationTokenSource.Token;
    private AsyncServiceScope Scope { get; set; }
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        _cancellationTokenSource.Dispose();
        _reservedPort.Dispose();
        return ValueTask.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;
        _reservedPort = new ReservedPort();
        var services = new ServiceCollection()
            .AddTestServices(protocol, _reservedPort.Port)
            .AddHostedService<OpcUaServerBackgroundService>();

        ServiceProvider = services.BuildServiceProvider();
        Scope = ServiceProvider.CreateAsyncScope();
        await Scope.ServiceProvider.GetRequiredService<IStartableEntityServer>().StartAsync();
        var serverStarted = Scope.ServiceProvider.GetService<IServerStartedHealthCheck>()!;
        await serverStarted;
        _isInitialized = true;
    }

    public T GetService<T>() where T : notnull
    {
        InitializeAsync().Wait(Token);
        return Scope.ServiceProvider.GetService<T>()!;
    }

    public T GetService<T>(Type t) where T : notnull
    {
        InitializeAsync().Wait(Token);
        var service = ServiceProvider.GetService<T>();
        if (!Equals(service, default(T)))
        {
            return service!;
        }

        return (T)Scope.ServiceProvider.GetService(t)!;
    }

    public async Task<ISession> CreateSession(string sessionId)
    {
        await InitializeAsync();
        return await Scope.ServiceProvider.GetService<IEntitySessionFactory>()!.GetSessionForAsync(sessionId, Token)
            .ThenAsync(e => e.Session);
    }

    public async Task<ISession> CreateSession() => await CreateSession(Guid.NewGuid().ToString());

    public async Task<TOut> ExecuteWithSession<T, TOut>(Func<ISession, T, TOut> execute)
    {
        var fixture = ServiceProvider.GetService<T>()!;
        ISession session = await CreateSession(Guid.NewGuid().ToString());
        return execute(session, fixture);
    }

    public async Task<TOut> ExecuteWithSessionAsync<T, TOut>(Func<ISession, T, Task<TOut>> execute)
    {
        var fixture = ServiceProvider.GetService<T>()!;
        ISession session = await CreateSession(Guid.NewGuid().ToString());
        return await execute(session, fixture);
    }

    public async Task ExecuteActionAsync(Func<ISession, IServiceProvider, Task> action) =>
        await action.Invoke(await CreateSession(), ServiceProvider);

    public async Task ExecuteActionAsync(Func<ISession, Task> action) =>
        await action.Invoke(await CreateSession());

    public async Task<T> ExecuteFunctionAsync<T>(Func<ISession, IServiceProvider, Task<T>> action) =>
        await action.Invoke(await CreateSession(), ServiceProvider);

    public async Task<T> ExecuteFunctionAsync<T>(Func<ISession, Task<T>> action) =>
        await action.Invoke(await CreateSession());
}

public sealed class ApplicationFixture<T> : ApplicationFixture where T : notnull
{
    private readonly Type _typeUnderTest;

    public ApplicationFixture(Type typeUnderTest, WebProtocol protocol) : base(protocol)
    {
        if (!typeUnderTest.IsAssignableTo(typeof(T)))
        {
            throw new ArgumentException($"Type {typeUnderTest.Name} is not assignable to {typeof(T).Name}");
        }

        _typeUnderTest = typeUnderTest;
    }

    public T TestedService => GetService<T>(_typeUnderTest);

    public override string ToString() => typeof(T).Name + "Fixture";

    public async Task<TOut> ExecuteWithSession<TOut>(Func<ISession, T, TOut> execute)
    {
        ISession session = await CreateSession(Guid.NewGuid().ToString());
        return execute(session, TestedService);
    }

    public async Task<TOut> ExecuteWithSessionAsync<TOut>(Func<ISession, T, Task<TOut>> execute)
    {
        ISession session = await CreateSession(Guid.NewGuid().ToString());
        return await execute(session, TestedService);
    }

    public async Task<TOut> ExecuteAsync<TOut>(Func<T, Task<TOut>> execute) => await execute.Invoke(TestedService);
}