using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging;

namespace Hoeyer.OpcUa.Client.Application.Events;

public interface IEntityMonitor<out T>
{
    Task<IMessageSubscription> SubscribeToChange(
        IMessageSubscriber<T> subscriber,
        CancellationToken cancellationToken = default);
}