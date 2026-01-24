using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Test.Simulation;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Test.Adapter.Client;

internal sealed class SpecifiedTestSession<T>(
    IServiceProvider provider,
    IEntitySession session,
    T serviceUndertest)
    : ISpecifiedTestSession<T>
{
    private readonly string _testName = typeof(T).Name;
    public T TestedService => serviceUndertest;
    public TOut ExecuteWithSession<TOut>(Func<IEntitySession, T, TOut> execute) => execute(session, serviceUndertest);

    public async Task<TOut> ExecuteWithSessionAsync<TOut>(Func<IEntitySession, T, Task<TOut>> execute) =>
        await execute(session, serviceUndertest);

    public async Task<TOut> ExecuteAsync<TOut>(Func<T, Task<TOut>> execute) => await execute.Invoke(serviceUndertest);

    public async Task ExecuteActionAsync(Func<IEntitySession, Task> action) =>
        await action.Invoke(session);

    public TWanted GetService<TWanted>() where TWanted : notnull => provider.GetService<TWanted>()!;

    public override string ToString() => _testName;
}