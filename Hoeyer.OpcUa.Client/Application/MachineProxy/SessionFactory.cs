using System.Threading.Tasks;
using Hoeyer.OpcUa.Configuration;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.MachineProxy;


public class SessionFactory(OpcUaApplicationOptions applicationOptions)
{
    private string _opcServerUrl = applicationOptions.ApplicationUri; 
    private string _machineStateNodeId = "ns=2;s=MachineStateSnapshot"; 
    public readonly ApplicationConfiguration Configuration = CreateApplicationConfig();
    
    public async Task<Session> CreateSessionAsync()    
    {
        var selectedEndpoint = CoreClientUtils.SelectEndpoint(_opcServerUrl, false);
        var endpointConfiguration = EndpointConfiguration.Create(Configuration);
        var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
        var session = await Session.Create(
            Configuration,
            endpoint,
            updateBeforeConnect: false, 
            sessionName: "OpcUaRemoteMachineProxy",
            sessionTimeout: 60000, 
            new UserIdentity(new AnonymousIdentityToken()),
            preferredLocales: null);
        
        return session;
    }

    public Session CreateSession() => CreateSessionAsync().Result;

    private static ApplicationConfiguration CreateApplicationConfig()
    {
        var config = new ApplicationConfiguration()
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