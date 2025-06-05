using System;
using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hoeyer.Common.Messaging.Subscriptions.ChannelBased;

public sealed class ChannelSubscriptionFactory<T> : IMessageSubscriptionFactory<T, ChannelBasedSubscription<T>>,
    IMessageSubscriptionFactory<T>
{
    private readonly ILogger _logger;
    private readonly IChannelSubscriptionCreationStrategy<T> _strategy;


    public ChannelSubscriptionFactory(ILogger logger)
    {
        _logger = logger;
        _strategy = new CreateUsingLoggerStrategy<T>(logger);
    }

    public ChannelSubscriptionFactory() : this(NullLogger.Instance)
    {
    }


    public ChannelSubscriptionFactory(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(typeof(ChannelSubscriptionFactory<T>));
        _strategy = new CreateUsingLoggerFactoryStrategy<T>(loggerFactory);
    }

    public ChannelBasedSubscription<T> CreateSubscription(
        IMessageUnsubscribable creator,
        IMessageConsumer<T> consumer)
    {
        var subscriptionId = Guid.NewGuid();
        _logger.LogInformation("Creating channel subscription {SubscriptionId}", subscriptionId);

        var unboundedChannelOptions = new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = true,
            SingleWriter = false,
            SingleReader = false
        };
        return _strategy.Create(creator, consumer, unboundedChannelOptions);
    }

    IMessageSubscription<T> IMessageSubscriptionFactory<T, IMessageSubscription<T>>
        .CreateSubscription(IMessageUnsubscribable creator, IMessageConsumer<T> consumer) =>
        CreateSubscription(creator, consumer);
}