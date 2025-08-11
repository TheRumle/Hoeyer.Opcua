using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Calling;
using Hoeyer.OpcUa.Client.Api.Calling.Exception;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Calling;

[OpcUaAgentService(typeof(IMethodCaller<>))]
public class MethodCaller<TAgent>(IAgentBrowser<TAgent> browser, IAgentSessionFactory factory)
    : IMethodCaller<TAgent>
{
    private static readonly string SessionClientId = typeof(TAgent).Name + "MethodCaller";

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
        var Agent = await browser.GetNodeStructure(token);
        var methodNodesByName = Agent.Methods;
        NodeId methodToCall = methodNodesByName[methodName] ??
                              throw new NoSuchAgentMethodException(Agent.AgentName, methodName);

        IAgentSession session = await factory.GetSessionForAsync(SessionClientId, token);
        return await session.Session.CallAsync(Agent.NodeId, methodToCall, token, args);
    }
}