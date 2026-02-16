using Hoeyer.OpcUa.Test.Simulation;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Test.Api;

public abstract class SimulationTestSession(Lazy<SimulationSetup> simulationSetup)
    : ISimulationTestSession
{
    public IServiceProvider ServiceProvider => simulationSetup.Value.ServiceProvider;

    public async ValueTask DisposeAsync() => await simulationSetup.Value.DisposeAsync();
    public async Task InitializeAsync() => await simulationSetup.Value.InitializeAsync();

    public async Task<ISession> GetOrCreateSession() =>
        (await simulationSetup.Value.SessionFactory.GetSessionForAsync(TestContext.Current!.Id!)).Session;

    public async Task ExecuteWithSession(Func<ISession, Task> action) =>
        await action.Invoke(await GetOrCreateSession());

    public async Task<T> ExecuteWithSession<T>(Func<ISession, Task<T>> action) =>
        await action.Invoke(await GetOrCreateSession());

    public T GetService<T>() where T : notnull => ServiceProvider.GetService<T>()!;

    public T GetService<T>(Type t) where T : notnull => (T)ServiceProvider.GetRequiredService(t);
}