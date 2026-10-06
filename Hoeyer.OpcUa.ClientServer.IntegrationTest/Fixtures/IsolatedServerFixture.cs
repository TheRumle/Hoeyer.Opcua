using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Fixtures.Server;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

public class IsolatedServerFixture : IAsyncInitializer
{
    private readonly string _id = Guid.NewGuid().ToString();

    private readonly IntegrationFixtureResources<IServerFixture> _resources =
        new(ServerFixtureAdapter.CreateOrGetCached);

    public IServiceProvider ServiceProvider => _resources.ServiceProvider;
    public IServerFixture ServerEnvironment => _resources.ServerEnvironment;

    public async Task InitializeAsync() => await _resources.InitializeAsync();

    public async Task<TOut> ExecuteWithSessionAsync<TOut>(Func<IEntitySession, IServiceProvider, Task<TOut>> execute) =>
        await execute(await OpenSession(), ServiceProvider);

    public async Task<TOut> ExecuteAsync<TOut>(Func<IServiceProvider, Task<TOut>> execute) =>
        await execute.Invoke(ServiceProvider);

    public async Task ExecuteActionAsync(Func<IEntitySession, Task> action) =>
        await action.Invoke(await OpenSession());

    public TWanted GetService<TWanted>() where TWanted : notnull => ServiceProvider.GetService<TWanted>()!;

    public async Task<IEntitySession> OpenSession() => await ServiceProvider
        .GetRequiredService<IEntitySessionFactory>().GetSessionAsync(_id);
}

public sealed class IntegrationTestFixture<T> : IAsyncInitializer
    where T : notnull
{
    private readonly string _id = Guid.NewGuid().ToString();

    private readonly IntegrationFixtureResources<IntegrationTestFixture<T>> _resources =
        new(ServerFixtureAdapter.CreateOrGetCached);

    private T? _serviceUnderTest;
    public IServiceProvider ServiceProvider => _resources.ServiceProvider;
    public IServerFixture ServerEnvironment => _resources.ServerEnvironment;

    public T TestedService => _serviceUnderTest;

    public async Task InitializeAsync()
    {
        await _resources.InitializeAsync();
        _serviceUnderTest = _resources.ServiceProvider.GetRequiredService<T>();
    }

    public async Task<TOut> ExecuteWithSessionAsync<TOut>(Func<IEntitySession, T, Task<TOut>> execute) =>
        await execute(await OpenSession(), _serviceUnderTest);

    public async Task<TOut> ExecuteAsync<TOut>(Func<T, Task<TOut>> execute) => await execute.Invoke(_serviceUnderTest);

    public async Task ExecuteActionAsync(Func<IEntitySession, Task> action) =>
        await action.Invoke(await OpenSession());

    public TWanted GetService<TWanted>() where TWanted : notnull => ServiceProvider.GetService<TWanted>();

    public async Task<IEntitySession> OpenSession() => await ServiceProvider
        .GetRequiredService<IEntitySessionFactory>().GetSessionAsync(_id);
}