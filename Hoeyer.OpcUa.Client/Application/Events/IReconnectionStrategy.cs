using System.Threading;
using System.Threading.Tasks;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Events;

public interface IReconnectionStrategy
{
    public Task<ISession> ReconnectIfNotConnected(ISession session, CancellationToken cancellationToken = default);
}