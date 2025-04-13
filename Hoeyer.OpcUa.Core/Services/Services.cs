using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Reflections;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

public static class Services
{
    public static OnGoingOpcEntityServiceRegistration AddOpcUaServerConfiguration(this IServiceCollection services,
        Func<IEntityServerConfigurationBuilder, IOpcUaEntityServerInfo> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke(EntityServerConfigurationBuilder.Create());
        services.AddSingleton(entityServerConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }

    public static OnGoingOpcEntityServiceRegistration WithEntityServices(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        var services = registration.Collection;
        var errs = OpcUaEntityServicesLoader.AddEntityServices(services)
            .Union(AddLoaders(services)).ToList();
        if (errs.Any()) throw new OpcUaEntityServiceConfigurationException(errs);
        
        AddInitializerServices(services);
        return registration;
    }

    private static IEnumerable<OpcUaEntityServiceConfigurationException> AddLoaders(IServiceCollection collection)
    {
        List<EntityServiceTypeContext> loaders = typeof(IEntityLoader<>)
            .GetConsumingAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .GetEntityServicesOfType(typeof(IEntityLoader<>))
            .ToList();

        var errs = NoLoadersImplementedErrors(loaders).ToList();
        if (errs.Count > 0) return errs;

        foreach (var loader in loaders)
        {
            collection.AddTransient(loader.ConcreteServiceType, loader.ImplementationType);
        }
        return [];
    }

    private static void AddInitializerServices(IServiceCollection services)
    {
        var initializerServices = OpcUaEntityTypes.Entities
            .Select(entity => typeof(EntityInitializer<>).MakeGenericType(entity))
            .ToList();

        foreach (var initializerService in initializerServices)
            services.AddSingleton(initializerService, initializerService);


        services.AddTransient(ConstructEnumerable);
        IEnumerable<IEntityInitializer> ConstructEnumerable(IServiceProvider p)
        {
            return initializerServices.Select(initializerService => p.GetService(initializerService) as IEntityInitializer ?? throw new OpcUaEntityServiceConfigurationException($"Trying to register {initializerService.Name} as an  {nameof(IEntityInitializer)}, but this is not possible."));
        }
    }

    private static IEnumerable<OpcUaEntityServiceConfigurationException> NoLoadersImplementedErrors(IEnumerable<EntityServiceTypeContext> loaders)
    {
        return OpcUaEntityTypes
            .Entities
            .Except(loaders.Select(e=>e.Entity))
            .Select(entityWithoutLoader => OpcUaEntityServiceConfigurationException.NoCustomImplementation(
                    entityWithoutLoader,
                    typeof(IEntityLoader<>).Name)
                );
    }
}