using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Microsoft.Extensions.Logging;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

public class CachedSessionFactory(
    EntitySessionFactory entitySessionFactory,
    ILogger<CachedSessionFactory> logger,
    ISubscriptionTransferStrategy subscriptionTransferStrategy,
    IReconnectionStrategy reconnectionStrategy) : IEntitySessionFactory
{
    private readonly ConcurrentDictionary<string, IEntitySession> _sessions = new();

    public async Task<IEntitySession> GetSessionAsync(string clientKey, CancellationToken token = default)
    {
        logger.BeginScope("SessionDetails: {Dictionary}", new Dictionary<string, object>
        {
            { "ClientKey", clientKey }
        });

        if (!_sessions.TryGetValue(clientKey, out IEntitySession? existingSession))
        {
            logger.LogInformation("No cache hit.");
            return _sessions[clientKey] = await entitySessionFactory.GetSessionAsync(clientKey, token);
        }

        if (SessionIsHealthy(existingSession))
        {
            logger.LogInformation("Session healthy and will be used");
            return existingSession;
        }

        logger.LogWarning("Session not healthy, attempting reconnect");

        if (await reconnectionStrategy.TryReconnect(existingSession.Session, token))
        {
            logger.LogInformation("Reconnection successful, the session will be used");
            return existingSession;
        }

        logger.LogWarning("Reconnection failed, a new session will be created..");

        var newSession = await entitySessionFactory.GetSessionAsync(clientKey, token);
        logger.LogInformation("New session successfully created, transferring subscriptions...");
        await subscriptionTransferStrategy.TransferSubscriptionsBetween(existingSession, newSession);
        _sessions[clientKey] = newSession;
        return newSession;
    }


    private static bool SessionIsHealthy(ISession session) =>
        session is { Disposed: false, Connected: true, KeepAliveStopped: false };

    private static bool SessionIsHealthy(IEntitySession session) => SessionIsHealthy(session.Session);
}