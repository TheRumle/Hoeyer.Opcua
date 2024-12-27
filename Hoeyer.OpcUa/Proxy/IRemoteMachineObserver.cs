using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Hoeyer.OpcUa.StateSnapshot;

namespace Hoeyer.OpcUa.Proxy;

public interface IRemoteMachineObserver<TEntity>
{
    public Task<Result<TEntity>> ReadEntityAsync(CancellationToken token);
    public StateChangeSubscription<ConnectionState> Subscribe(IStateChangeSubscriber<ConnectionState> subscriber);
}
