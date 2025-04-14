using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Application.Reading;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Reflections;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client.Services;

public static class ServicesExtensions
{
    public static OnGoingOpcEntityServiceRegistration WithOpcUaClientServices(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        var services = registration.Collection;
        services.AddTransient<INodeTreeTraverser, BreadthFirstStrategy>();
        services.AddTransient<INodeBrowser, NodeBrowser>();
        services.AddTransient<INodeReader, NodeReader>();
        services.AddTransient<IEntitySessionFactory, SessionFactory>();
        
        
        var entities = typeof(OpcUaEntityAttribute)
            .GetConsumingAssemblies()
            .SelectMany(e => e.GetTypes())
            .Where(e => e.IsAnnotatedWith<OpcUaEntityAttribute>())
            .ToHashSet();
        
        var genericMatcher = typeof(EntityDescriptionMatcher<>);
        foreach (var m in entities)
        {
            var instantiatedMatcher = genericMatcher.MakeGenericType(m);
            services.AddTransient(instantiatedMatcher, (_) => DefaultMatcherFactory.CreateMatcher(m));
        }

        var serviceContextGroups = (from serviceContext in ConstructServiceContextFor(entities)
                group serviceContext by serviceContext.ServiceType).ToList();

        AssertAllEntitiesHaveAllServices(serviceContextGroups, entities);
        
        foreach (var context in serviceContextGroups.SelectMany(e=>e)) context.AddToCollection(services);
        
        return registration;
    }

    private static void AssertAllEntitiesHaveAllServices(List<IGrouping<Type, EntityServiceTypeContext>> serviceContextGroups, HashSet<Type> entities)
    {
        var errs = new List<OpcUaEntityServiceConfigurationException>();
        foreach (var g in serviceContextGroups)
        {
            var serviceType = g.Key;
            var groupServices = g.ToList();
            if (groupServices.Count == entities.Count)
            {
                continue;
            }

            var entitiesWithTypeContext = groupServices.Select(e => e.Entity).ToHashSet();
            errs.AddRange(entities
                .Where(entity => !entitiesWithTypeContext.Contains(entity))
                .Select(entity => new OpcUaEntityServiceConfigurationException($"Cannot register service {serviceType.Name} for entity '{entity.Name}'")));
        }

        if (errs.Any()) throw new OpcUaEntityServiceConfigurationException(errs);
    }


    private static IEnumerable<EntityServiceTypeContext> ConstructServiceContextFor(IEnumerable<Type> entityTypes)
    {
        var entityTypesList = entityTypes.ToList(); 
        HashSet<Type> serviceTypes = typeof(IEntityBrowser) //marker type
            .Assembly
            .ExportedTypes
            .Where(e => e.IsAnnotatedWith<OpcUaEntityServiceAttribute>())
            .ToHashSet();

        AssertClientServices(serviceTypes);

        return from entity in entityTypesList
            from service in serviceTypes
            from result in service.ConstructEntityServices(entity)
            select result;
    }

    private static void AssertClientServices(IEnumerable<Type> clientTypes)
    {
        List<OpcUaEntityServiceConfigurationException> errors = new();
        foreach (var clientType in clientTypes)
        {
            var interfaces = clientType.GetInterfaces()
                .Where(e => e.IsGenericType && e.GenericTypeArguments.Length == 1).ToList();
            if (interfaces.Count != 1)
            {
                errors.Add(new OpcUaEntityServiceConfigurationException(
                    $"Classes annotated with {nameof(OpcUaEntityServiceAttribute)} must implement an interface with exactly one type parameter to decide which entity it relates to."));
            }
        }

        if (errors.Any()) throw new OpcUaEntityServiceConfigurationException(errors);

    }


    private static IEnumerable<EntityServiceTypeContext> ConstructEntityServices(
        this Type serviceImplementation,
        Type entity)
    {
        if (!serviceImplementation.IsGenericTypeDefinition 
            || serviceImplementation.GetGenericArguments().Length != 1 
            || !entity.IsClass)
        {
            throw new OpcUaEntityServiceConfigurationException(
                "The specified type does not represent a uninstantiated generic type definition. The service type must take 1 generic argument, must be a non-abstract class, and must not be a type representing an instantiation of the generic type definition.");
        }

        var attribute = serviceImplementation.GetCustomAttribute<OpcUaEntityServiceAttribute>();
        var instantiatedServiceImpl = serviceImplementation.MakeGenericType(entity);
        yield return new EntityServiceTypeContext(instantiatedServiceImpl, attribute.ServiceType, entity, attribute.Lifetime);
    }
    
}