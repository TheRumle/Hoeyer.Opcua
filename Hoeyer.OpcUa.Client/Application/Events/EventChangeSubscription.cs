using System;
using Hoeyer.Common.Messaging;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Events;

public sealed record EventChangeSubscription(Subscription Subscription, IMessageSubscription MessageSubscription) : IMessageSubscription
{
    public void Dispose()
    {
        Subscription.Dispose();
        MessageSubscription.Dispose();
    }

    public Guid SubscriptionId => MessageSubscription.SubscriptionId;
    public uint OpcUaId => Subscription.Id;

    public bool IsCancelled => MessageSubscription.IsCancelled || Subscription.PublishingStopped;

    public bool IsPaused => MessageSubscription.IsPaused;
    public IMessageSubscription MessageSubscription { get; } = MessageSubscription;
    public Subscription Subscription { get; } = Subscription;

    public void Unpause()
    {
        Subscription.PublishingEnabled = true;
        MessageSubscription.Unpause();
    }

    public void Pause()
    {
        Subscription.PublishingEnabled = false;
        MessageSubscription.Pause();
    }
}