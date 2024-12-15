using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.Machines.OpcUa.Application.MachineProxy;


public class SessionFactory
{
    public readonly ApplicationConfiguration _config = CreateApplicationConfig();
    private string _opcServerUrl = "opc.tcp://localhost:4840"; 
    private string _machineStateNodeId = "ns=2;s=MachineStateSnapshot"; 
    private ApplicationConfiguration _applicationConfig = CreateApplicationConfig();
    
    public async Task<Session> CreateSessionAsync()    
    {
        var selectedEndpoint = CoreClientUtils.SelectEndpoint(_opcServerUrl, false);
        var endpointConfiguration = EndpointConfiguration.Create(_config);
        var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
        var _session = await Session.Create(_config, endpoint, false, "OpcUaRemoteMachineProxy", 60000, new UserIdentity(new AnonymousIdentityToken()), null);
        return _session;
    }

    public Session CreateSession()
    {
        var selectedEndpoint = CoreClientUtils.SelectEndpoint(_opcServerUrl, false);
        var endpointConfiguration = EndpointConfiguration.Create(_config);
        var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
        return Session.Create(_config, endpoint, false, "OpcUaRemoteMachineProxy", 60000, new UserIdentity(new AnonymousIdentityToken()), null).Result;
    }
    
    private static ApplicationConfiguration CreateApplicationConfig()
    {
        return new ApplicationConfiguration
        {
            ApplicationName = "OpcUaRemoteMachineProxy",
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier(),
                AutoAcceptUntrustedCertificates = true
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration
            {
                OutputFilePath = "./OpcUaRemoteMachineProxy.log",
                DeleteOnLoad = false,
                TraceMasks = 0x1
            }
        };
    }
}