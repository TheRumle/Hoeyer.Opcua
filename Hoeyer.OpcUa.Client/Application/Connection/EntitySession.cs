using System.Collections.Generic;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

internal sealed class AgentSession(ISession session) : IAgentSession
{
    private readonly List<AgentSubscription> _managedAgentSubscriptions = new();

    public ISession Session => session;
    public IEnumerable<AgentSubscription> AgentSubscriptions => _managedAgentSubscriptions;

    public void Dispose()
    {
        session.Dispose();
        foreach (AgentSubscription? agentSubscription in AgentSubscriptions)
        {
            agentSubscription.Dispose();
        }

        _managedAgentSubscriptions.Clear();
    }

    public static ISession ToISession(AgentSession agentSession) => agentSession.Session;
}