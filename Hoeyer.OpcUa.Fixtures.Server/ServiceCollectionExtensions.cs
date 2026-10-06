using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Fixtures.Server;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServerTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] entityAssemblyMarkers
    ) =>
        services
            .AddCoreTestServices(args, entityAssemblyMarkers)
            .WithOpcUaServer(entityAssemblyMarkers)
            .Collection;
}