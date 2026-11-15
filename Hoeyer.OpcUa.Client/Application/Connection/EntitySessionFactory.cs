using Hoeyer.Common.Utilities.Providers;
using Hoeyer.OpcUa.Client.Abstractions.Configuration;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

internal sealed class EntitySessionFactory(
    SessionEndpointFactory endpointFactory,
    ICultureInfoProvider cultureInfoProvider,
    IClientApplicationConfigurationFactory configurationFactory,
    ISessionFactory sessionFactory,
    ILogger<EntitySessionFactory> logger)
    : IEntitySessionFactory
{
    public Task<IEntitySession> GetSessionAsync(
        string clientKey,
        CancellationToken token = default) =>
        CreateSession(clientKey, token);

    private async Task<IEntitySession> CreateSession(
        string sessionName,
        CancellationToken token)
    {
        using var logScope = logger.BeginScope("Creating session {SessionName}", sessionName);
        ApplicationConfiguration config = configurationFactory.CreateClientConfiguration();
        
        logger.LogDebug("Validating configuration...");
        await config.ValidateAsync(ApplicationType.Client, token);

        var endpoint = await endpointFactory.CreateRootEndpoint(token);
        var session = await sessionFactory.CreateAsync(
            config,
            endpoint,
            false,
            sessionName,
            1_000_000,
            new UserIdentity(new AnonymousIdentityToken()),
            [cultureInfoProvider.CurrentCulture().TwoLetterISOLanguageName],
            token);

        session.ReturnDiagnostics =
            DiagnosticsMasks.LocalizedText
            | DiagnosticsMasks.InnerDiagnostics
            | DiagnosticsMasks.All;

        logger.LogInformation(
            "Session created for sessionName '{Client}'",
            sessionName);

        return new EntitySession(session);
    }
}