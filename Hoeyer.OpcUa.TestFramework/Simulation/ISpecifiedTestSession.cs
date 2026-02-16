using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Test.Api;

namespace Hoeyer.OpcUa.Test.Simulation;

/// <summary>
///     A contexts for <see cref="ISimulationTestSession" /> providing methods for
/// </summary>
/// <seealso cref="ISimulationTestSession" />
/// <typeparam name="T"></typeparam>
public interface ISpecifiedTestSession<out T>
{
    public T TestedService { get; }
    TOut ExecuteWithSession<TOut>(Func<IEntitySession, T, TOut> execute);
    Task<TOut> ExecuteWithSessionAsync<TOut>(Func<IEntitySession, T, Task<TOut>> execute);
    Task<TOut> ExecuteAsync<TOut>(Func<T, Task<TOut>> execute);
    Task ExecuteActionAsync(Func<IEntitySession, Task> action);
    TWanted GetService<TWanted>() where TWanted : notnull;
}