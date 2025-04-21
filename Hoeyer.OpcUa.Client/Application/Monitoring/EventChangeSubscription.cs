using System;
using Hoeyer.Common.Messaging;
using Hoeyer.Common.Messaging.Api;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

public sealed class EventChangeSubscription(Subscription Subscription, IMessageSubscription MessageSubscription) : IMessageSubscription
{
    public void Dispose()
    {
        Subscription.Dispose();
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

    /// <inheritdoc />
    public void Cancel() => MessageSubscription.Dispose();
}