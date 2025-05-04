using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
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
        var errs = OpcUaEntityServicesLoader
            .AddEntityServices(services)
            .Union(AddLoaders(services))
            .ToList();
        if (errs.Any()) throw new OpcUaEntityServiceConfigurationException(errs);
        

        return registration;
    }

    private static IEnumerable<OpcUaEntityServiceConfigurationException> AddLoaders(IServiceCollection collection)
    {
        var loaderType = typeof(IEntityLoader<>);
        
        var loaders = loaderType
            .GetConsumingAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(TypeFilter)
            .Select(type =>
            {
                Type? serviceType = GetServiceType(type);
                Type? entity = serviceType?.GetGenericArguments()[0];
                if (entity == null) return default;
                if (!entity.IsAnnotatedWith<OpcUaEntityAttribute>()) return default;
                
                return new EntityServiceTypeContext(type, serviceType!, entity, ServiceLifetime.Transient);
            })
            .Where(interfaceImpl => interfaceImpl != default)
            .ToList();

        var errs = NoLoadersImplementedErrors(loaders).ToList();
        if (errs.Count > 0) return errs;

        foreach (var loader in loaders)
        {
            loader.AddToCollection(collection);
        }
        return [];
        
        bool TypeFilter(Type type) => type is { IsClass: true, IsAbstract: false, IsNested: false, IsPublic: true } && type.GetInterfaces().Any();
        Type? GetServiceType(Type type) => type.GetInterfaces().FirstOrDefault(e => e.IsGenericType && loaderType == e.GetGenericTypeDefinition());
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