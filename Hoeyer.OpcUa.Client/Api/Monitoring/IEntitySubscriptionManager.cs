using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using Hoeyer.Common.Messaging.Api;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IEntitySubscriptionManager<out T>
{
    public Subscription? Subscription { get; }
    
    Task<IMessageSubscription> SubscribeToChange(
        IMessageConsumer<T> consumer,
        CancellationToken cancellationToken = default);
}