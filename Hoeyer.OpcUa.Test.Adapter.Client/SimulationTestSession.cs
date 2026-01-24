using Hoeyer.OpcUa.Test.Simulation;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Test.Adapter.Client;

public abstract class SimulationTestSession(SimulationSetup simulationSetup)
    : ISimulationTestSession
{
    public IServiceProvider ServiceProvider => simulationSetup.ServiceProvider;

    public async ValueTask DisposeAsync() => await simulationSetup.DisposeAsync();
    public async Task InitializeAsync() => await simulationSetup.InitializeAsync();

    public async Task<ISession> GetOrCreateSession() =>
        (await simulationSetup.SessionFactory.GetSessionForAsync(TestContext.Current!.Id!)).Session;

    public async Task ExecuteWithSession(Func<ISession, Task> action) =>
        await action.Invoke(await GetOrCreateSession());

    public async Task<T> ExecuteWithSession<T>(Func<ISession, Task<T>> action) =>
        await action.Invoke(await GetOrCreateSession());

    public T GetService<T>() where T : notnull => ServiceProvider.GetService<T>()!;

    public T GetService<T>(Type t) where T : notnull => (T)ServiceProvider.GetRequiredService(t);
}