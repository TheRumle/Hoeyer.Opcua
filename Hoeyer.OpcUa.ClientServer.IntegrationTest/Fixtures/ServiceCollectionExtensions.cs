using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.Server.Configuration;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

public static class ServiceCollectionExtensions
{
    public static OnGoingOpcEntityServiceRegistrationWithModels AddClientTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] clientModelMarker) =>
        services.AddClientTestServices(args, clientModelMarker, clientModelMarker);

    public static OnGoingOpcEntityServiceRegistrationWithModels AddClientTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] entityAssemblyMarkers,
        Type[] clientModelMarker)
    {
        return services.AddCoreTestServices(args, entityAssemblyMarkers)
            .WithOpcUaClientConfiguration(clientModelMarker,
                c => { c.WithEntitySessionFactory<CachedSessionFactory>(); });
    }


    public static void AddClientAndServerTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] entityAssemblyMarkers,
        Type[] clientModelMarker,
        Type[] serverModelMarker)
    {
        var clientServices = services.AddClientTestServices(args, entityAssemblyMarkers, clientModelMarker);
        clientServices.WithOpcUaServer(serverModelMarker);
    }

    public static OnGoingOpcEntityServerServiceRegistration AddClientAndServerTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] entityAssemblyMarkers)
    {
        var serverServices = services.AddClientTestServices(args, entityAssemblyMarkers, entityAssemblyMarkers)
            .WithOpcUaServer(entityAssemblyMarkers);
        return serverServices;
    }
}