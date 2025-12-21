using Hoeyer.OpcUa.Client.Api.Connection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;

public sealed class SimulationContext<T>(IServiceProvider provider, IEntitySession session, T serviceUndertest)
{
    private readonly string _testName = typeof(T).Name;
    public readonly string TestedServiceName = typeof(T).Name;
    public T TestedService => serviceUndertest;
    public TOut ExecuteWithSession<TOut>(Func<IEntitySession, T, TOut> execute) => execute(session, serviceUndertest);

    public async Task<TOut> ExecuteWithSessionAsync<TOut>(Func<IEntitySession, T, Task<TOut>> execute) =>
        await execute(session, serviceUndertest);

    public async Task<TOut> ExecuteAsync<TOut>(Func<T, Task<TOut>> execute) => await execute.Invoke(serviceUndertest);


    public async Task ExecuteActionAsync(Func<IEntitySession, Task> action) =>
        await action.Invoke(session);

    public override string ToString() => _testName;

    public TWanted GetService<TWanted>() where TWanted : notnull => provider.GetService<TWanted>()!;
}