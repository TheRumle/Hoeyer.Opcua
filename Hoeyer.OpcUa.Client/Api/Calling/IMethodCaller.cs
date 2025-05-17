using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Client.Api.Calling;

public interface IMethodCaller
{
    public Task<IList<object>> CallMethod(string methodName, CancellationToken token = default, params object[] args);
}

public interface IMethodCaller<TEntity>;