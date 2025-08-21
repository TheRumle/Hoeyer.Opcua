using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Client.Api.Calling;

public interface IMethodCaller<TEntity>
{
    public Task CallMethod(string methodName, CancellationToken token = default, params object[] args);
    public Task<T> CallMethod<T>(string methodName, CancellationToken token = default, params object[] args);
}