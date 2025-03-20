using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Hoeyer.OpcUa.Core.Entity.State;
using Hoeyer.OpcUa.Core.Observation;

namespace Hoeyer.OpcUa.Core.Proxy;

public interface IEntityObserver<TEntity>
{
    public Task<Result<TEntity>> ReadEntityAsync(CancellationToken token);
    public StateChangeSubscription<ConnectionState> Subscribe(IStateChangeSubscriber<ConnectionState> subscriber);
}