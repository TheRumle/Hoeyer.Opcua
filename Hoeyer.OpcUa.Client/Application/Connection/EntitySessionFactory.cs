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
        using var logScope = logger.BeginScope("Creating session {0}", sessionName);
        var config = configurationFactory.CreateClientConfiguration();

        logger.LogDebug("Validating configuration...");
        await config.ValidateAsync(ApplicationType.Client, token);

        var session = await ConnectToServer(sessionName, token, config);

        session.ReturnDiagnostics =
            DiagnosticsMasks.LocalizedText | DiagnosticsMasks.InnerDiagnostics | DiagnosticsMasks.All;

        logger.LogInformation("Session created for sessionName '{Client}'", sessionName);
        return new EntitySession(session);
    }

    private async Task<ISession> ConnectToServer(string sessionName, CancellationToken token,
        ApplicationConfiguration config)
    {
        Endpoint ??= CreateEndpoint(config);
        logger.LogInformation("Using configuration {0} to create session", Endpoint.Description.ToLoggingObject());
        try
        {
            return await sessionFactory.CreateAsync(
                config,
                Endpoint,
                false,
                sessionName,
                1000000,
                new UserIdentity(new AnonymousIdentityToken()),
                null,
                token
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create session");
            throw;
        }
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