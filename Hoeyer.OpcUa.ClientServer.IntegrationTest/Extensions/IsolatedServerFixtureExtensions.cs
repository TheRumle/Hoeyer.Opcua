using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;

namespace Hoeyer.OpcUa.IntegrationTest.Extensions;

public static class IsolatedServerFixtureExtensions
{
    extension<T>(IsolatedServerFixture<T> fixture) where T : notnull
    {
        public async Task<TOut> ExecuteAsync<TOut>(Func<T, Task<TOut>> execute) => await execute.Invoke(fixture.TestedService);
        public async Task ExecuteActionAsync(Func<IEntitySession, Task> action)
        {
            using var session = await fixture.OpenSession();
            await action.Invoke(session);
        }

        public async Task<TOut> ExecuteWithSessionAsync<TOut>(Func<IEntitySession, T, Task<TOut>> execute)
        {
            using var session = await fixture.OpenSession();
            return await execute(session, fixture.TestedService);
        }
    }
    
    extension(IsolatedServerFixture fixture)
    {
        public async Task<TOut> ExecuteWithSessionAsync<TOut>(Func<IEntitySession, IServiceProvider, Task<TOut>> execute) =>
            await execute(await fixture.OpenSession(), fixture.ServiceProvider);

        public async Task ExecuteActionAsync(Func<IEntitySession, Task> action) =>
            await action.Invoke(await fixture.OpenSession());

        public async Task<TOut> ExecuteAsync<TOut>(Func<IServiceProvider, Task<TOut>> execute) =>
            await execute.Invoke(fixture.ServiceProvider);
    }
}