using System.Collections.Generic;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Server.Application;

[OpcUaEntityService(typeof(IEntityChangedSubscriptionManager<>), ServiceLifetime.Singleton)]
public sealed class EntityChangedSubscriptionManager<T>(IEntityTranslator<T> translator, ILogger? logger = null) 
    : IEntityChangedSubscriptionManager<T>
{
    private readonly SubscriptionManager<T> _subscriptionManager = new();
    public void Publish(IEntityNode message) => _subscriptionManager.Publish(translator.Translate(message));
    public void Publish(T message) => _subscriptionManager.Publish(message);

    public void Dispose()
    {
        _subscriptionManager.Dispose();
    }

    public IEnumerable<MessageSubscription> Subscribers { get; }

    public MessageSubscription Subscribe(IMessageConsumer<T> subscriber) => _subscriptionManager.Subscribe(subscriber);
    public void Unpause() => _subscriptionManager.Unpause();
    public void Pause() => _subscriptionManager.Pause();
}


