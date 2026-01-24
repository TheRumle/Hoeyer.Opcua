using Opc.Ua.Client;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Test.Simulation;

/// <summary>
///     Represents a simulation test session containing its own <see cref="ISession" /> and isolated environment the
///     session can connect to.
///     Provides a proxy for executing actions using the opened <see cref="ISession" /> and for getting different services.
/// </summary>
public interface ISimulationTestSession : IAsyncDisposable, IAsyncInitializer
{
    public T GetService<T>() where T : notnull;
    public T GetService<T>(Type t) where T : notnull;
    public Task<ISession> GetOrCreateSession();
    public Task ExecuteWithSession(Func<ISession, Task> action);
    public Task<T> ExecuteWithSession<T>(Func<ISession, Task<T>> action);
}