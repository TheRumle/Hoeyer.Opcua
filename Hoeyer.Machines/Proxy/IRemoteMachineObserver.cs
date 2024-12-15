using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Hoeyer.Machines.StateSnapshot;

namespace Hoeyer.Machines.Proxy;

public interface IRemoteMachineObserver<TEntity>
{
    public Task<Result<TEntity>> ReadEntityAsync(CancellationToken token);
    public StateChangeSubscription<ConnectionState> Subscribe(IStateChangeSubscriber<ConnectionState> subscriber);
}
