using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions.ChannelBased;

internal interface IChannelSubscriptionCreationStrategy<T>
{
    public IMessageSubscription<T> Create(
        IMessageUnsubscribable creator,
        IMessageConsumer<T> consumer,
        UnboundedChannelOptions unboundedChannelOptions);
}