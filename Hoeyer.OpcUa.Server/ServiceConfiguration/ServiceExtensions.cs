using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Application.Node.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

public record OnGoingOpcEntityServerServiceRegistration(IServiceCollection Collection) : OnGoingOpcEntityServiceRegistration(Collection)
{}

public static class ServiceExtensions
{
    public static OnGoingOpcEntityServerServiceRegistration WithEntityServerArgs(
        this OnGoingOpcEntityServiceRegistration registration, Action<ServerConfiguration> additionalConfiguration)
    {
        registration.Collection.AddSingleton<OpcUaEntityServerConfigurationSetup>(p =>
        {
            var standardConfig = p.GetService<OpcUaEntityServerConfiguration>()!;
            if (standardConfig == null) throw new InvalidOperationException($"No {nameof(OpcUaEntityServerConfiguration)} has been registered! This should be prevented using builder pattern! SHOULD NOT HAPPEN!" );
            return new OpcUaEntityServerConfigurationSetup(standardConfig, additionalConfiguration);
        });

        return new OnGoingOpcEntityServerServiceRegistration(registration.Collection);
    }
    
    
    public static OnGoingOpcEntityServiceRegistration WithEntityNodeGeneration(this OnGoingOpcEntityServiceRegistration services)
    {
        services.Collection.AddSingleton<OpcUaEntityServerFactory>(p =>
        {
            var loggerFactory = p.GetService<ILoggerFactory>()!;
            var configuration = p.GetService<OpcUaEntityServerConfigurationSetup>();
            if (configuration is null) throw new InvalidOperationException($"No {nameof(OpcUaEntityServerConfiguration)} has been configured!");
            return new OpcUaEntityServerFactory(configuration, GetEntityNodeCreators(), new EntityNodeManagerFactory(loggerFactory));
        });

        return services;
    }

    private static IEnumerable<IEntityNodeCreator> GetEntityNodeCreators()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IEntityNodeCreator).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Where(e => e.GetConstructor(Type.EmptyTypes) != null)
            .Select(Activator.CreateInstance)
            .Cast<IEntityNodeCreator>()
            .ToList();//eager init
    }
}