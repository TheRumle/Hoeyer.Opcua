using System;
using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging.Subscriptions.ChannelBased;

public sealed class CreateUsingLoggerFactoryStrategy<TContent>(ILoggerFactory factory)
    : IChannelSubscriptionCreationStrategy<TContent>
{
    public IMessageSubscription<TContent> Create(
        IMessageUnsubscribable creator,
        IMessageConsumer<TContent> consumer,
        UnboundedChannelOptions unboundedChannelOptions)
    {
        var subscriptionId = Guid.NewGuid();
        ILogger logger = factory.CreateLogger($"Subscription{subscriptionId.ToString()}");
        Channel<IMessage<TContent>> channel = Channel.CreateUnbounded<IMessage<TContent>>(unboundedChannelOptions);
        return new ChannelBasedSubscription<TContent>(subscriptionId, creator, consumer, channel, logger);
    }
}