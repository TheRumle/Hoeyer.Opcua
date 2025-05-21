using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Calling.Exception;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Calling;

[OpcUaEntityService(typeof(IMethodCaller<>))]
public class MethodCaller<TEntity>(IEntityBrowser<TEntity> browser, IEntitySessionFactory factory)
    : IMethodCaller<TEntity>
{
    public async Task CallMethod(string methodName,
        CancellationToken token = default,
        params object[] args)
    {
        await CallNode(methodName, token, args);
    }

    /// <inheritdoc />
    public async Task<T> CallMethod<T>(string methodName, CancellationToken token = default, params object[] args)
    {
        IList<object> res = await CallNode(methodName, token, args);
        return (T)res[0];
    }


    private async Task<IList<object>> CallNode(string methodName, CancellationToken token = default,
        params object[] args)
    {
        IEntityNode entityNode = browser.LastState?.node ?? await browser.BrowseEntityNode(token);
        Dictionary<string, MethodState> methodNodesByName = entityNode.MethodsByName;
        MethodState methodToCall = methodNodesByName[methodName]
                                   ?? throw new NoSuchEntityMethodException(entityNode.BaseObject.BrowseName.Name,
                                       methodName);

        ISession session = await factory.CreateSessionAsync("MySession", token);
        return await session.CallAsync(entityNode.BaseObject.NodeId, methodToCall.NodeId, token, args);
    }
}