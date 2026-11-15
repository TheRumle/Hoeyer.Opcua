using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Abstractions.Discovery;

public interface IEndpointDiscovery 
{
    Task<IEnumerable<EndpointDescription>> FindSupportedEndpointsAsync(Uri uri, CancellationToken cancellationToken = default);
}