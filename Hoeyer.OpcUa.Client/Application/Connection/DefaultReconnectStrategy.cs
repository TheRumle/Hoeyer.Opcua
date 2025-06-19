using System;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Connection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

internal sealed class DefaultReconnectStrategy : IReconnectionStrategy
{
    /// <inheritdoc />
    public async Task<ISession> ReconnectIfNotConnected(ISession session, CancellationToken cancellationToken = default)
    {
        if (session.Connected)
        {
            return session;
        }

        await session.ReconnectAsync(cancellationToken);
        return session;
    }

    /// <inheritdoc />
    public async Task<bool> TryReconnect(ISession session, CancellationToken cancellationToken = default)
    {
        try
        {
            await ReconnectIfNotConnected(session, cancellationToken);
            return true;
        }
        catch (Exception _)
        {
            return false;
        }
    }
}