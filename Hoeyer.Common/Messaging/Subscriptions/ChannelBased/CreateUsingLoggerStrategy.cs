using System;
using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging.Subscriptions.ChannelBased;

public sealed class CreateUsingLoggerStrategy<TContent>(ILogger logger) : IChannelSubscriptionCreationStrategy<TContent>
{
    public IMessageSubscription<TContent> Create(
        IMessageUnsubscribable creator,
        IMessageConsumer<TContent> consumer,
        UnboundedChannelOptions unboundedChannelOptions)
    {
        var subscriptionId = Guid.NewGuid();
        Channel<IMessage<TContent>> channel = Channel.CreateUnbounded<IMessage<TContent>>(unboundedChannelOptions);
        return new ChannelBasedSubscription<TContent>(subscriptionId, creator, consumer, channel, logger);
    }
}