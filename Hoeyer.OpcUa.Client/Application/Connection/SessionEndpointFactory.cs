using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.Client.Abstractions.Configuration;
using Hoeyer.OpcUa.Client.Abstractions.Discovery;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Connection;

internal sealed class SessionEndpointFactory(
    IEndpointDiscovery endpointDiscovery,
    ClientApplicationConfiguration configuration
)
{
    public async Task<ConfiguredEndpoint> CreateRootEndpoint(CancellationToken cancellationToken)
    {
        var endpoints = await endpointDiscovery
            .FindSupportedEndpointsAsync(configuration.ApplicationNamespaceUri, cancellationToken);
        
        return endpoints
            .MapFirst(endpoint => new ConfiguredEndpoint(
                null,
                endpoint,
                EndpointConfiguration.Create(configuration)
            )) ?? throw new InvalidOperationException("No matching OPC UA endpoint was discovered.");
        
    }
}