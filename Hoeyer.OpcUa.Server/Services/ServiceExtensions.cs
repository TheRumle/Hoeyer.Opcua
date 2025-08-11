using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Services;

public static class ServiceExtensions
{
    public static OnGoingOpcAgentServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcAgentServiceRegistration serviceRegistration,
        Action<ServerConfiguration>? additionalConfiguration = null)
    {
        IServiceCollection collection = serviceRegistration.Collection;

        collection.AddSingleton<OpcUaAgentServerSetup>(p =>
        {
            var standardConfig = p.GetService<IOpcUaAgentServerInfo>();
            if (standardConfig == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(IOpcUaAgentServerInfo)} has been registered! This should be prevented using builder pattern. Are you using the library as intended and using the {nameof(Core.Services.Services.AddOpcUaServerConfiguration)} {nameof(IServiceCollection)} extension method?");
            }

            return new OpcUaAgentServerSetup(standardConfig, additionalConfiguration ?? (value => { }));
        });


        collection.AddSingleton<IAgentAccessConfigurator, NoAccessRestrictionsConfigurator>();
        collection.AddSingleton<AgentServerStartedMarker>();
        collection.AddSingleton<OpcUaAgentServerFactory>();
        collection.AddSingleton<OpcAgentServer>();
        collection.AddSingleton<IStartableAgentServer>(p =>
        {
            var factory = p.GetRequiredService<OpcUaAgentServerFactory>();
            return factory.CreateServer();
        });
        AddLoaders(serviceRegistration.Collection);


        return new OnGoingOpcAgentServerServiceRegistration(serviceRegistration.Collection);
    }

    public static OnGoingOpcAgentServerServiceRegistration WithOpcUaServerAsBackgroundService(
        this OnGoingOpcAgentServiceRegistration serviceRegistration,
        Action<ServerConfiguration>? additionalConfiguration = null)
    {
        var serverConfig = serviceRegistration.WithOpcUaServer(additionalConfiguration);
        serverConfig.Collection.AddHostedService<OpcUaServerBackgroundService>();
        return serverConfig;
    }

    public static OpcUaAgentServerSetup WithAdditionalServerConfiguration(IOpcUaAgentServerInfo setup,
        Action<ServerConfiguration> additionalConfiguration)
    {
        return new OpcUaAgentServerSetup(setup, additionalConfiguration);
    }

    private static void AddLoaders(IServiceCollection collection)
    {
        Type loaderType = typeof(IAgentLoader<>);
        IEnumerable<(Type Service, Type Implementation)> loaders = OpcUaAgentTypes
            .TypesFromReferencingAssemblies
            .Select(type =>
            {
                Type? foundLoaderInterface = type
                    .GetInterfaces()
                    .FirstOrDefault(@interface => @interface.Namespace == loaderType.Namespace
                                                  && @interface.IsConstructedGenericType &&
                                                  @interface.GetGenericTypeDefinition() == loaderType);

                if (foundLoaderInterface is null) return default;
                return (Service: foundLoaderInterface, Implementation: type);
            })
            .Where(result => result.Service is not null);

        foreach ((Type service, Type implementation) in loaders)
        {
            collection.AddSingleton(service, implementation);
        }
    }
}