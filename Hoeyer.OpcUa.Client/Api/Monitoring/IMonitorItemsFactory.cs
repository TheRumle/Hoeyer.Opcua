using System.Threading;
using System.Threading.Tasks;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IMonitorItemsFactory<T>
{
    Task<(Subscription subscription, MonitoredItem variableMonitoring)> GetOrCreate(ISession session, CancellationToken cancellationToken = default);
    Task<(Subscription subscription, MonitoredItem variableMonitoring)> Create(ISession session, CancellationToken cancellationToken);
}