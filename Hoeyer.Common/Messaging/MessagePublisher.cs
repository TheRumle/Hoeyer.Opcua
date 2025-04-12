using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging;

public sealed class MessagePublisher<T>(ILogger<MessagePublisher<T>> logger) : ISubscribable<T>
{
    private static string MessageName = typeof(T).Name;
    public int NumberOfSubscriptions => Subscriptions.Count;
    private ConcurrentDictionary<Guid, Subscription<T>> Subscriptions { get; set; } = new();

    public void Publish(T message)
    {
        foreach (var kvp in Subscriptions)
        {
            var id = kvp.Key;
            var sub = kvp.Value;

            if (sub.IsCancelled)
            {
                logger.LogInformation("Removing subscription {Id}", sub.SubscriptionId);
                if (!Subscriptions.TryRemove(id, out _))
                {
                    logger.LogWarning("Failed to remove subscription {Id}", id);
                }
                continue;
            }

            if (sub.IsActive)
            {
                sub.ForwardMessage(message);
            }
        }
    }
    

    
    [Pure]
    public Subscription<T> Subscribe(IMessageSubscriber<T> stateChangeSubscriber)
    {
        logger.BeginScope("Subscribing to messages of type '" + MessageName + '\'');
        var subscription = new Subscription<T>(stateChangeSubscriber);
        if (!Subscriptions.TryAdd(subscription.SubscriptionId, subscription))
        {
            logger.LogError("Failed to add subscription with for {@StateChangeSubscriber}. Messages will not be forwarded...", stateChangeSubscriber);
        }
        else
        {
            logger.LogError("Added subscription {Id}", subscription.SubscriptionId);
        }
        return subscription;
    }
}