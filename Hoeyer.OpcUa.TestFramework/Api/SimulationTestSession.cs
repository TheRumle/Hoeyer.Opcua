using Hoeyer.OpcUa.Test.Simulation;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Test.Api;

public class SimulationTestSession(SimulationSetup simulationSetup) : ISimulationTestSession
{
    private bool _disposed;
    private bool _initialized;
    public IServiceProvider ServiceProvider => simulationSetup.ServiceProvider;
    public Guid TestSessionId => SimulationSetup.SimulationTestIdentity;
    public SimulationSetup SimulationSetup => simulationSetup;

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await simulationSetup.DisposeAsync();
        _disposed = true;
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await simulationSetup.InitializeAsync();
        _initialized = true;
    }

    public async Task<ISession> GetOrCreateSession() =>
        (await simulationSetup.SessionFactory.GetSessionForAsync(TestSessionId.ToString())).Session;

    public async Task ExecuteWithSession(Func<ISession, Task> action) =>
        await action.Invoke(await GetOrCreateSession());

    public async Task<T> ExecuteWithSession<T>(Func<ISession, Task<T>> action) =>
        await action.Invoke(await GetOrCreateSession());

    public T GetService<T>() where T : notnull => ServiceProvider.GetService<T>()!;

    public T GetService<T>(Type t) where T : notnull => (T)ServiceProvider.GetRequiredService(t);
}