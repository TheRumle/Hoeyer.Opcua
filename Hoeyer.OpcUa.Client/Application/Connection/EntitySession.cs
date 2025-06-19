using System.Collections.Generic;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

internal sealed class EntitySession(ISession session) : IEntitySession
{
    private readonly List<EntitySubscription> _managedEntitySubscriptions = new();

    public ISession Session => session;
    public IEnumerable<EntitySubscription> EntitySubscriptions => _managedEntitySubscriptions;

    public void Dispose()
    {
        session.Dispose();
        foreach (EntitySubscription? entitySubscription in EntitySubscriptions)
        {
            entitySubscription.Dispose();
        }

        _managedEntitySubscriptions.Clear();
    }

    public static ISession ToISession(EntitySession entitySession) => entitySession.Session;

    public EntitySubscription CreateSubscription()
    {
        var s = new EntitySubscription(this);
        _managedEntitySubscriptions.Add(s);
        return s;
    }
}