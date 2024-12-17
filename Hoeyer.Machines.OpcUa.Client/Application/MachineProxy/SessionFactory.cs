using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.Machines.OpcUa.Client.Application.MachineProxy;


public class SessionFactory
{
    public readonly ApplicationConfiguration Configuration = CreateApplicationConfig();
    private string _opcServerUrl = "opc.tcp://localhost:4840/freeopcua/server/"; 
    private string _machineStateNodeId = "ns=2;s=MachineStateSnapshot"; 
    private ApplicationConfiguration _applicationConfig = CreateApplicationConfig();
    
    public async Task<Session> CreateSessionAsync()    
    {
        var selectedEndpoint = CoreClientUtils.SelectEndpoint(_opcServerUrl, false);
        var endpointConfiguration = EndpointConfiguration.Create(_applicationConfig);
        var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
        var session = await Session.Create(
            _applicationConfig,
            endpoint,
            false, // Do not use reconnect
            "OpcUaRemoteMachineProxy", // Session name
            60000, // Session timeout in milliseconds
            new UserIdentity(new AnonymousIdentityToken()), // Anonymous authentication
            null); // No session locale
        
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