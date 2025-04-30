using System;
using System.Linq;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Services;

public static class ServiceExtensions
{
    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistration serviceRegistration,
        Action<ServerConfiguration>? additionalConfiguration = null)
    {
        
        serviceRegistration.Collection.AddSingleton<OpcUaEntityServerSetup>(p =>
        {
            var standardConfig = p.GetService<IOpcUaEntityServerInfo>();
            if (standardConfig == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(IOpcUaEntityServerInfo)} has been registered! This should be prevented using builder pattern. Are you using the library as intended and using the {nameof(Core.Services.Services.AddOpcUaServerConfiguration)} {nameof(IServiceCollection)} extension method?");
            }

            return new OpcUaEntityServerSetup(standardConfig, additionalConfiguration ?? (value => { }));
        });

        serviceRegistration.Collection.AddSingleton<IEntityNodeAccessConfigurator, NoAccessRestrictionsConfigurator>();
        serviceRegistration.Collection.AddSingleton<IDomainMasterManagerFactory, DomainMasterNodeManagerFactory>();
        serviceRegistration.Collection.AddSingleton<EntityServerStartedMarker>();
        serviceRegistration.Collection.AddSingleton<OpcUaEntityServerFactory>();
        serviceRegistration.Collection.AddSingleton<OpcEntityServer>();
        serviceRegistration.Collection.AddSingleton<IStartableEntityServer>(p =>
        {
            var factory = p.GetRequiredService<OpcUaEntityServerFactory>();
            return factory.CreateServer();
        });
        
        return new OnGoingOpcEntityServerServiceRegistration(serviceRegistration.Collection);
    }

    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServerAsBackgroundService(
        this OnGoingOpcEntityServiceRegistration serviceRegistration,
        Action<ServerConfiguration>? additionalConfiguration = null)
    {
        var serverConfig = serviceRegistration.WithOpcUaServer(additionalConfiguration);
        serverConfig.Collection.AddHostedService<OpcUaServerBackgroundService>();
        return serverConfig;
    }
    
    public static OpcUaEntityServerSetup WithAdditionalServerConfiguration(IOpcUaEntityServerInfo setup,
        Action<ServerConfiguration> additionalConfiguration)
    {
        return new OpcUaEntityServerSetup(setup, additionalConfiguration);
    }
}