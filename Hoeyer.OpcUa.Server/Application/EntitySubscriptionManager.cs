using System.Collections.Generic;
using FluentResults;
using Hoeyer.OpcUa.Core.Application.Observation;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Core.Observation;
using Hoeyer.OpcUa.Server.Entity.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;
using Subscription = Opc.Ua.Client.Subscription;

namespace Hoeyer.OpcUa.Server.Application;


public interface IEntitySubscriptionManager : ISubscriptionManager
{
    public Result Subscribe(Subscription subscription);
    public Result Unsubscribe(Subscription subscription);
    
}

public class EntitySubscriber() : IStateChangeSubscriber<IEntityNode>
{
    /// <inheritdoc />
    public void OnStateChange(IEntityNode stateChange)
    {
        throw new System.NotImplementedException();
    }
}

public delegate void OnEntityChanged(IEntityNode node);
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