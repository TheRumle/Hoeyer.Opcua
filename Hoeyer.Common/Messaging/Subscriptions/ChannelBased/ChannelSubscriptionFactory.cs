using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging.Subscriptions.ChannelBased;

public sealed class ChannelSubscriptionFactory<T>(ILogger<ChannelSubscriptionFactory<T>> logger)
    : IMessageSubscriptionFactory<T>
{
    public IMessageSubscription<T> CreateSubscription(IMessageConsumer<T> consumer,
        Action<IMessageSubscription<T>>? disposeCallBack = null) => Create(consumer, disposeCallBack);

    public ChannelBasedSubscription<T> CreateSubscription(IMessageConsumer<T> consumer,
        Action<ChannelBasedSubscription<T>>? disposeCallBack = null) =>
        Create(consumer, disposeCallBack);

    private ChannelBasedSubscription<T> Create(IMessageConsumer<T> consumer,
        Action<ChannelBasedSubscription<T>>? disposeCallBack)
    {
        var subscriptionId = Guid.NewGuid();
        logger.LogDebug("Creating channel subscription '{SubscriptionId}' for {Consumer}", subscriptionId,
            consumer.GetType().Name);

        var unboundedChannelOptions = new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = true,
            SingleWriter = false,
            SingleReader = false
        };

        var channel = Channel.CreateUnbounded<IMessage<T>>(unboundedChannelOptions);
        return new ChannelBasedSubscription<T>(subscriptionId, consumer, channel, logger, disposeCallBack);
    }
}