using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Hoeyer.OpcUa.Entity.State;

namespace Hoeyer.OpcUa.Proxy;

public interface IEntityObserver<TEntity>
{
    public Task<Result<TEntity>> ReadEntityAsync(CancellationToken token);
    public StateChangeSubscription<ConnectionState> Subscribe(IStateChangeSubscriber<ConnectionState> subscriber);
}