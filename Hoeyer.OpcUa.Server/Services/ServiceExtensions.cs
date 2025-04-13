using System;
using System.Linq;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Configuration;
using Hoeyer.OpcUa.Server.Entity.Management;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Services;

public static class ServiceExtensions
{
    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistration serviceRegistration,
        Action<ServerConfiguration>? additionalConfiguration = null)
    {
        serviceRegistration.Collection.AddSingleton<IEntityNodeManagerFactory, EntityNodeManagerFactory>();
        serviceRegistration.Collection.AddSingleton<OpcUaEntityServerSetup>(p =>
        {
            var standardConfig = p.GetService<IOpcUaEntityServerInfo>();
            if (standardConfig == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(IOpcUaEntityServerInfo)} has been registered! This should be prevented using builder pattern. Are you using the library as intended and using the {nameof(OpcUa.Core.Services.Services.AddOpcUaServerConfiguration)} {nameof(IServiceCollection)} extension method?");
            }

            return new OpcUaEntityServerSetup(standardConfig, additionalConfiguration ?? (value => { }));
        });

        serviceRegistration.Collection.AddSingleton<IDomainMasterManagerFactory>(p =>
        {
            var nodeManagers = p.GetService<IEntityNodeManagerFactory>();
            var setup = p.GetService<IOpcUaEntityServerInfo>();
            return new DomainMasterNodeManagerFactory(nodeManagers!, setup!);
        });

        serviceRegistration.Collection.AddSingleton<EntityServerStartedMarker>();
        serviceRegistration.Collection.AddSingleton<OpcUaEntityServerFactory>();
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
}