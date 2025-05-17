using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Server.Api;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class ApplicationFixture : IAsyncDisposable, IAsyncInitializer
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IServiceCollection _collection;

    public ApplicationFixture(IServiceCollection collection)
    {
        _collection = collection;
    }

    public ApplicationFixture() : this(new AllOpcUaServicesFixture().ServiceCollection)
    {
    }

    public CancellationToken Token => _cancellationTokenSource.Token;
    public IServiceScope Scope { get; private set; } = null!;
    public IServiceProvider ServiceProvider { get; private set; }


    /// <inheritdoc />  
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        _cancellationTokenSource.Dispose();
        return ValueTask.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        ServiceProvider = _collection.BuildServiceProvider();
        Scope = ServiceProvider.CreateAsyncScope();
        await Scope.ServiceProvider.GetRequiredService<IStartableEntityServer>().StartAsync();
        var serverStarted = Scope.ServiceProvider.GetService<EntityServerStartedMarker>()!;
        await serverStarted;
    }

    public T GetService<T>() where T : notnull => Scope.ServiceProvider.GetService<T>()!;

    public T GetService<T>(Type t) where T : notnull
    {
        var s = ServiceProvider.GetService<T>();
        if (!Equals(s, default(T))) return s;
        return (T)Scope.ServiceProvider.GetService(t)!;
    }

    public async Task<ISession> CreateSession(string sessionId)
    {
        return await Scope.ServiceProvider.GetService<IEntitySessionFactory>()!.CreateSessionAsync(sessionId);
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
}

public sealed class ApplicationFixture<T> : ApplicationFixture where T : notnull
{
    private readonly ServiceDescriptor _valueDescriptor;

    public ApplicationFixture(ServiceDescriptor descriptor, IServiceCollection collection) : base(collection)
    {
        _valueDescriptor = descriptor;
    }

    public T TestedService => (T)ServiceProvider.GetService(_valueDescriptor.ImplementationType!);

    /// <inheritdoc />
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

    public async Task<TOut> ExecuteAsync<TOut>(Func<T, Task<TOut>> execute)
    {
        return await execute.Invoke(TestedService);
    }
}