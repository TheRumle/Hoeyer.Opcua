using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Calling;
using Hoeyer.OpcUa.Client.Api.Calling.Exception;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Calling;

public class MethodCaller<TEntity>(IEntityBrowser<TEntity> browser, IEntitySessionFactory factory)
    : IMethodCaller<TEntity>
{
    private static readonly string SessionClientId = typeof(TEntity).Name + "MethodCaller";

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
        var returnValue = res[0];
        return returnValue == null ? default! : (T)OpcToCSharpValueParser.ParseOpcValue(returnValue)!;
    }


    private async Task<IList<object>> CallNode(string methodName, CancellationToken token = default,
        params object[] args)
    {
        EntityNodeStructure entityNode = await browser.GetNodeStructure(token);
        IReadOnlyDictionary<string, NodeId> methodNodesByName = entityNode.Methods;
        NodeId methodToCall = methodNodesByName[methodName] ??
                              throw new NoSuchEntityMethodException(entityNode.EntityName, methodName);

        IEntitySession session = await factory.GetSessionForAsync(SessionClientId, token);
        return await session.Session.CallAsync(entityNode.NodeId, methodToCall, token, args);
    }
}