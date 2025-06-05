using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public interface IMessageSubscriptionFactory<T> : IMessageSubscriptionFactory<T, IMessageSubscription<T>>;

public interface IMessageSubscriptionFactory<out T, out TSubscriptionType>
    where TSubscriptionType : IMessageSubscription<T>
{
    public TSubscriptionType CreateSubscription(
        IMessageUnsubscribable creator,
        IMessageConsumer<T> consumer);
}