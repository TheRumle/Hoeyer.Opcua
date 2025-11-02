using System;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Subscriptions;

namespace Hoeyer.OpcUa.Client.Services;

public sealed class StateChangeObserver<T>(
    Lazy<(Task<IMessageSubscription> subscriptionTask, ICurrentEntityStateChannel<T> currentEntityStateChannel)>
        lazyCreation)
    : IStateChangeObserver<T>
{
    public async Task<ISubscribedStateChangeMonitor<T>> BeginObserveAsync()
    {
        var (subscription, currentEntityStateChannel) = lazyCreation.Value;
        return new SubscribedStateChangeMonitor<T>(currentEntityStateChannel.Reader, await subscription);
    }
}