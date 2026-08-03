using System.Collections.Generic;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Client.Abstractions.Monitoring;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

internal sealed class EntitySession(ISession session) : IEntitySession
{
    private readonly IReadOnlyList<EntitySubscription> _managedEntitySubscriptions = [];

    public ISession Session => session;
    public IEnumerable<EntitySubscription> EntitySubscriptions => _managedEntitySubscriptions;

    public void Dispose()
    {
        session.Dispose();
        foreach (EntitySubscription entitySubscription in EntitySubscriptions)
        {
            entitySubscription.Dispose();
        }
    }

    public static ISession ToISession(EntitySession entitySession) => entitySession.Session;
}