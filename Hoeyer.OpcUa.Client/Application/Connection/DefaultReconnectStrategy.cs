using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Connection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

public sealed class DefaultReconnectStrategy() : IReconnectionStrategy
{
    /// <inheritdoc />
    public async Task<ISession> ReconnectIfNotConnected(ISession session, CancellationToken cancellationToken = default)
    {
        if (session.Connected) return session;
        await session.ReconnectAsync(cancellationToken);
        return session;
    }
}