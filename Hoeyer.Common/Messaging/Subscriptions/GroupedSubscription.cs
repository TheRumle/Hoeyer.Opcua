using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public sealed record GroupedSubscription<T> : MessageSubscription, IMessageSubscription<T>
{
    /// <inheritdoc />
    public GroupedSubscription(IMessageUnsubscribable creator) : base(creator)
    {
    }

    /// <inheritdoc />
    public void Forward(IMessage<T> message)
    {
        throw new System.NotImplementedException();
    }
}