using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public interface IMessageSubscriptionFactory<T>
{
    public IMessageSubscription<T> CreateSubscription(
        IMessageUnsubscribable creator,
        IMessageConsumer<T> consumer);
}