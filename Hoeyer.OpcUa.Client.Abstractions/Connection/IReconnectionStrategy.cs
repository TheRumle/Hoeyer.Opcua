using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Abstractions.Connection;

public interface IReconnectionStrategy
{
    public Task<ISession> ReconnectIfNotConnected(ISession session, CancellationToken cancellationToken = default);

    public Task<bool> TryReconnect(ISession session, CancellationToken cancellationToken = default);
}