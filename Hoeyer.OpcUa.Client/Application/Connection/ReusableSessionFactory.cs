using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

internal class ReusableSessionFactory(
    ILogger<ReusableSessionFactory> logger,
    IOpcUaTargetServerInfo applicationOptions,
    ISubscriptionTransferStrategy subscriptionTransferStrategy,
    IReconnectionStrategy reconnectionStrategy)
    : IEntitySessionFactory
{
    private readonly ApplicationConfiguration _configuration = CreateApplicationConfig();

    private readonly ConcurrentDictionary<string, EntitySession> _sessions = new();
    private ConfiguredEndpoint _endpoint => CreateEndpoint();

    public async Task<IEntitySession> GetSessionForAsync(string clientKey, CancellationToken token = default)
    {
        if (!_sessions.TryGetValue(clientKey, out EntitySession? existingSession))
        {
            return _sessions[clientKey] = await CreateSession(clientKey, token);
        }

        if (SessionIsHealthy(existingSession))
        {
            return existingSession;
        }

        if (await reconnectionStrategy.TryReconnect(existingSession.Session, token))
        {
            return existingSession;
        }

        EntitySession newSession = await CreateSession(clientKey, token);
        await subscriptionTransferStrategy.TransferSubscriptionsBetween(existingSession, newSession);
        _sessions[clientKey] = newSession;
        return newSession;
    }


    private ConfiguredEndpoint CreateEndpoint()
    {
        var opcServerUrl = applicationOptions.ApplicationNamespace.ToString();
        var endpointConfiguration = EndpointConfiguration.Create(_configuration);

        var endpoint = new EndpointDescription
        {
            EndpointUrl = opcServerUrl,
            SecurityMode = MessageSecurityMode.None,
            SecurityPolicyUri = SecurityPolicies.None,
            UserIdentityTokens = new UserTokenPolicy[]
            {
                new()
                {
                    PolicyId = "Anonymous",
                    TokenType = UserTokenType.Anonymous
                }
            }
        };

        return new ConfiguredEndpoint(null, endpoint, endpointConfiguration);
    }

    private async Task<EntitySession> CreateSession(string client, CancellationToken token)
    {
        var session = await Session.Create(
            _configuration,
            _endpoint,
            false,
            client,
            60000,
            new UserIdentity(new AnonymousIdentityToken()),
            null,
            token);

        logger.LogInformation("Session created for client '{Client}'", client);
        session.ReturnDiagnostics = DiagnosticsMasks.LocalizedText |
                                    DiagnosticsMasks.InnerDiagnostics;
        return new EntitySession(session);
    }

    private static bool SessionIsHealthy(ISession session) =>
        session is { Disposed: false, Connected: true, KeepAliveStopped: false };

    private static bool SessionIsHealthy(IEntitySession session) => SessionIsHealthy(session.Session);

    private static ApplicationConfiguration CreateApplicationConfig()
    {
        var config = new ApplicationConfiguration
        {
            ApplicationName = "OpcUaClientApp",
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier(),
                AutoAcceptUntrustedCertificates = true, // Allow untrusted certificates
                AddAppCertToTrustedStore = true
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration()
        };

        return config;
    }
}