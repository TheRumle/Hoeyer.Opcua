using Hoeyer.OpcUa.Client.Abstractions.Configuration;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Client.Extensions;
using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

public sealed class EntitySessionFactory(
    IApplicationConfigurationRequirements applicationOptions,
    IClientApplicationConfigurationFactory configurationFactory,
    ISessionFactory sessionFactory,
    ILogger<EntitySessionFactory> logger)
    : IEntitySessionFactory
{
    public ConfiguredEndpoint? Endpoint { get; private set; }

    public Task<IEntitySession> GetSessionAsync(string clientKey, CancellationToken token = default) =>
        CreateSession(clientKey, token);

    private async Task<IEntitySession> CreateSession(string sessionName, CancellationToken token)
    {
        using var logScope = logger.BeginScope("Creating new session with name {0}", sessionName);
        var config = configurationFactory.CreateClientConfiguration();

        logger.LogDebug("Validating configuration...");
        await config.ValidateAsync(ApplicationType.Client, token);

        logger.LogInformation("Creating session with name {SessionName}", sessionName);

        Endpoint ??= CreateEndpoint(config);
        logger.LogInformation("Using configuration {0}", Endpoint.Description.ToLoggingObject());

        var session = await sessionFactory.CreateAsync(
            config,
            Endpoint,
            false,
            sessionName,
            (uint)config.ClientConfiguration.DefaultSessionTimeout,
            new UserIdentity(new AnonymousIdentityToken()),
            preferredLocales: null,
            token
        );

        session.ReturnDiagnostics =
            DiagnosticsMasks.LocalizedText | DiagnosticsMasks.InnerDiagnostics | DiagnosticsMasks.All;

        logger.LogInformation("Session created for sessionName '{Client}'", sessionName);
        return new EntitySession(session);
    }


    private ConfiguredEndpoint CreateEndpoint(ApplicationConfiguration configuration)
    {
        var opcServerUrl = applicationOptions.ApplicationNamespace.ToString();
        var endpointConfiguration = EndpointConfiguration.Create(configuration);

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

        logger.LogInformation("Created endpoint: {endpoint}", endpoint.ToLoggingObject());
        return new ConfiguredEndpoint(null, endpoint, endpointConfiguration);
    }
}