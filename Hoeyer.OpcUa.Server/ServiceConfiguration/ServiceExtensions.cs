using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

public record OnGoingOpcEntityServerServiceRegistration(IServiceCollection Collection) : OnGoingOpcEntityServiceRegistration(Collection)
{}

public static class ServiceExtensions
{
    public static OnGoingOpcEntityServerServiceRegistration AddEntityOpcUaServer(this OnGoingOpcEntityServiceRegistration serviceRegistration,  Action<ServerConfiguration>? additionalConfiguration = null)
    {
        serviceRegistration.Collection.AddSingleton<OpcUaEntityServerSetup>(p =>
        {
            var standardConfig = p.GetService<IOpcUaEntityServerConfiguration>()!;
            if (standardConfig == null) throw new InvalidOperationException($"No {nameof(IOpcUaEntityServerConfiguration)} has been registered! This should be prevented using builder pattern! SHOULD NOT HAPPEN!" );
            return new OpcUaEntityServerSetup(standardConfig, additionalConfiguration ?? ((value) => { }));
        });
        
        serviceRegistration.Collection.AddSingleton<OpcUaEntityServerFactory>(p =>
        {
            var loggerFactory = p.GetService<ILoggerFactory>()!;
            var configuration = p.GetService<OpcUaEntityServerSetup>();
            if (configuration is null) throw new InvalidOperationException($"No {nameof(IOpcUaEntityServerConfiguration)} has been configured!");
            return new OpcUaEntityServerFactory(configuration, [], loggerFactory);
        });
        return new OnGoingOpcEntityServerServiceRegistration(serviceRegistration.Collection);
    }
    
    
    /// <summary>
    /// Scans assembly for implementations of <see cref="IEntityNodeCreator"/> and passes them to the EntityServer. 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static OnGoingOpcEntityServiceRegistration WithAutomaticEntityNodeCreation(this OnGoingOpcEntityServerServiceRegistration services)
    {
        services.Collection.AddSingleton<OpcUaEntityServerFactory>(p =>
        {
            var loggerFactory = p.GetService<ILoggerFactory>()!;
            var configuration = p.GetService<OpcUaEntityServerSetup>();
            if (configuration is null) throw new InvalidOperationException($"No {nameof(IOpcUaEntityServerConfiguration)} has been configured!");
            return new OpcUaEntityServerFactory(configuration, GetEntityNodeCreators(), loggerFactory);
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