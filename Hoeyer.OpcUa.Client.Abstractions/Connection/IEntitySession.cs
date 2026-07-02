using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Client.Abstractions.Monitoring;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Abstractions.Connection;

public interface IEntitySession : IDisposable
{
    public ISession Session { get; }
    public IEnumerable<EntitySubscription> EntitySubscriptions { get; }
}