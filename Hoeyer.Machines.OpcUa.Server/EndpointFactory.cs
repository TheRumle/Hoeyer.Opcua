using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Server;

public class EndpointFactory(string endpointUrl)
{
    public ConfiguredEndpoint CreateEndpoint(ApplicationConfiguration configuration)
    {
        var endpointDescription = new EndpointDescription
        {
            EndpointUrl = endpointUrl,
            SecurityMode = MessageSecurityMode.None,
            SecurityPolicyUri = SecurityPolicies.None
        };

        EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);
        var endpoint = new ConfiguredEndpoint(new(), endpointDescription, endpointConfiguration);
        return endpoint;
    }
}