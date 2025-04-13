using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Application.Observation;

namespace Hoeyer.OpcUa.Core.Proxy;

public interface IEntityObserver<TEntity>
{
    public Task<TEntity> ReadEntityAsync(CancellationToken token);
    public StateChangeSubscription<ConnectionState> Subscribe(IStateChangeSubscriber<ConnectionState> subscriber);
}