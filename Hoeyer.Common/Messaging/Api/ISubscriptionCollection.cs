using System;
using System.Collections.Generic;

namespace Hoeyer.Common.Messaging.Api;

public interface ISubscriptionCollection<T> : ISubscribable<T>
{
    public int ActiveSubscriptionsCount { get; }
    IEnumerable<IMessageSubscription<T>> Subscriptions { get; }

    void Remove(Guid messageSubscription);
}