using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.MachineProxy;

internal class SessionFactory(IOpcUaEntityServerInfo applicationOptions) : IEntitySessionFactory
{
    private readonly string _opcServerUrl = applicationOptions.OpcUri.ToString();
    private ApplicationConfiguration Configuration { get; } = CreateApplicationConfig();

    public async Task<ISession> CreateSessionAsync(string sessionId)
    {
        var selectedEndpoint = CoreClientUtils.SelectEndpoint(_opcServerUrl, false);
        
        var endpointConfiguration = EndpointConfiguration.Create(Configuration);
        var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
        var session = await Session.Create(
            Configuration,
            endpoint,
            updateBeforeConnect: false,
            sessionId,
            60000,
            new UserIdentity(new AnonymousIdentityToken()),
            null);
        session.TransferSubscriptionsOnReconnect = true;
        session.DeleteSubscriptionsOnClose = false;

        return session;
    }

    /// <inheritdoc />
    public ISession CreateSession(string sessionId) => CreateSessionAsync(sessionId).Result;

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