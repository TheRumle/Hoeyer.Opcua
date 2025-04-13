using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Application.Observation;
using Hoeyer.OpcUa.Core.Entity.Node;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Application;

internal sealed class EntitySubscriptionManager(ILogger logger, IEntityNode node) : StateChangeSubscriptionManager<IEntityNode>(node)
{
    
    /// <inheritdoc />
    public IList<Opc.Ua.Server.Subscription> GetSubscriptions()
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public event SubscriptionEventHandler? SubscriptionCreated;

    /// <inheritdoc />
    public event SubscriptionEventHandler? SubscriptionDeleted;

    /// <inheritdoc />
    public StateChangeSubscription<IEntityNode> Subscribe(IStateChangeSubscriber<IEntityNode> stateChangeSubscriber)
    {
        throw new System.NotImplementedException();
    }
}