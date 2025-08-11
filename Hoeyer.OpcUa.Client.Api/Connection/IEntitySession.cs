using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Connection;

public interface IAgentSession : IDisposable
{
    public ISession Session { get; }
    public IEnumerable<AgentSubscription> AgentSubscriptions { get; }
}