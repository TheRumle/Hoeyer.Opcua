using Hoeyer.OpcUa.Client.Api.Connection;

namespace Hoeyer.OpcUa.Test.Simulation;

public interface ISpecifiedTestSessionForFaked<out TService, out T>
    where T : class
{
    public T TestDataInstance { get; }
    public TService TestedService { get; }
    public IEntitySession Session { get; }
    TOut ExecuteWithSession<TOut>(Func<IEntitySession, T, TService, TOut> execute);
    Task<TOut> ExecuteWithSessionAsync<TOut>(Func<IEntitySession, T, TService, Task<TOut>> execute);
    Task<TOut> ExecuteAsync<TOut>(Func<T, TService, Task<TOut>> execute);
    Task ExecuteActionAsync(Func<IEntitySession, T, TService, Task> action);
    TWanted GetService<TWanted>() where TWanted : notnull;
}