using System;
using System.Collections.Generic;

namespace Hoeyer.Common.Messaging.Api;

public interface ISubscriptionCollection<T> : IDisposable, ISubscribable<T>
{
    public int NumberOfSubscriptions { get; }
    IEnumerable<IMessageSubscription<T>> Subscriptions { get; }
    
    void Remove(Guid messageSubscription);
}