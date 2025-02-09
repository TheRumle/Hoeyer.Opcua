using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

public sealed class OpcServerServiceCollection(IServiceCollection collection)
{
    internal readonly IServiceCollection Collection = collection;
}