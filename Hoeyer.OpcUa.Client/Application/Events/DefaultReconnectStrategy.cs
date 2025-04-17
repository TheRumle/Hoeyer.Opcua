using System.Threading;
using System.Threading.Tasks;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Events;

public sealed class DefaultReconnectStrategy : IReconnectionStrategy
{
    /// <inheritdoc />
    public async Task<ISession> ReconnectIfNotConnected(ISession session, CancellationToken cancellationToken = default)
    {
        if (session.Connected) return session;
        await session.ReconnectAsync(cancellationToken);
        return session;
    }
}