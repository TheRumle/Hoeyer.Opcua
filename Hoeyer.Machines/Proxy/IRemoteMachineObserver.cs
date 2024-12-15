using System.Threading;
using System.Threading.Tasks;
using FluentResults;

namespace Hoeyer.Machines.Proxy;

public interface IRemoteMachineObserver<TEntity>
{
    public Task<Result<TEntity>> ReadEntityAsync(CancellationToken token);

}
