using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.Machines.Proxy;

public interface IRemoteMachineObserver<TEntity>
{
    public Task<TEntity> ReadEntityAsync(CancellationToken token);

}
