using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IEntityMonitor<out T>
{
    Task<IMessageSubscription> SubscribeToChange(
        IMessageConsumer<T> consumer,
        CancellationToken cancellationToken = default);
}