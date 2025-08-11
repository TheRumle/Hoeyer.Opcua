using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging.Api;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IAgentSubscriptionManager<out T>
{
    public Subscription? Subscription { get; }

    Task<IMessageSubscription> SubscribeToAllPropertyChanges(
        IMessageConsumer<T> consumer,
        CancellationToken cancellationToken = default);

    Task<IMessageSubscription> SubscribeToProperty(
        IMessageConsumer<T> consumer,
        string propertyName,
        CancellationToken cancellationToken = default);
}