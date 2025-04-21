using System.Collections.Generic;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

[OpcUaEntityService(typeof(IEntityChangedBroadcaster<>), ServiceLifetime.Singleton)]
public sealed class EntityChangedBroadcaster<T>(IEntityTranslator<T> translator) : IEntityChangedBroadcaster<T>
{
    private readonly SubscriptionManager<T> _entitySubscriptionManager = new();
    private readonly SubscriptionManager<IEntityNode> _entityNodeSubscription = new();
    private readonly StateChangePublisher<PropertyState, object> _stateChangePublisher = new();
    
    public IMessageSubscription<IEntityNode> Subscribe(IMessageConsumer<IEntityNode> subscriber) => _entityNodeSubscription.Subscribe(subscriber);
    public IMessageSubscription<IEnumerable<StateChange<PropertyState, object>>> Subscribe(IMessageConsumer<IEnumerable<StateChange<PropertyState, object>>> subscriber) => _stateChangePublisher.Subscribe(subscriber);
    

    public void Publish((IEntityNode NewState, IEnumerable<StateChange<PropertyState, object>> Changes) message)
    {
        var entity = translator.Translate(message.NewState);
        _stateChangePublisher.Publish(message.Changes);
        _entityNodeSubscription.Publish(message.NewState);
        _entitySubscriptionManager.Publish(entity);
    }
    
    public ISubscribable<T> EntitySubcribable => _entitySubscriptionManager;
}


