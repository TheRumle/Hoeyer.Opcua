using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Application.Reading;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Reflections;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client;

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
        
        var clientTypes = typeof(IEntityBrowser)//marker type
            .Assembly
            .ExportedTypes
            .Where(e => TypeExtensions.IsAnnotatedWith<ClientServiceAttribute>(e))
            .ToList();

        var entities = typeof(OpcUaEntityAttribute)
            .GetConsumingAssemblies()
            .SelectMany(e => e.GetTypes())
            .Where(e => e.IsAnnotatedWith<OpcUaEntityAttribute>())
            .ToList();

        var (serviceTypeContexts, errors) = ConstructTypeContexts(clientTypes, entities, registration.Collection);
        if (errors.Count > 0) throw new OpcUaEntityServiceConfigurationException(errors);

        foreach (var context in serviceTypeContexts)
        {
            services.AddTransient(context.ConcreteServiceType, context.ImplementationType);
        }


        var genericMatcher = typeof(EntityDescriptionMatcher<>);
        foreach (var m in entities)
        {
            var instantiatedMatcher = genericMatcher.MakeGenericType(m);
            services.AddTransient(instantiatedMatcher, p =>
                DefaultMatcherFactory.CreateMatcher(m));
        }

        return registration;
    }


    private static (List<EntityServiceTypeContext> contexts,
        List<OpcUaEntityServiceConfigurationException> errors) 
        ConstructTypeContexts(IEnumerable<Type> clientTypes, IEnumerable<Type> entities,  IServiceCollection serviceCollection)
    {
        List<OpcUaEntityServiceConfigurationException> errors = new ();
        List<EntityServiceTypeContext> contexts = new ();
        foreach (var clientType in clientTypes)
        {
            var enumerable = entities as Type[] ?? entities.ToArray();
            var interfaces = clientType.GetInterfaces()
                .Where(e => e.IsGenericType && e.GenericTypeArguments.Length == 1).ToList();
            if (interfaces.Count != 1)
            {
                errors.Add(new OpcUaEntityServiceConfigurationException($"Classes annotated with {nameof(ClientServiceAttribute)} must implement an interface with exactly one type parameter to decide which entity it relates to."));
                continue;
            }
            
            foreach (var entity in enumerable)
            {
                var serviceType = interfaces[0]!.GetGenericTypeDefinition();
                var parameterizedImplementation = clientType.MakeGenericType(entity);
                serviceCollection.AddTransient(parameterizedImplementation, parameterizedImplementation);
                
                contexts.Add(new EntityServiceTypeContext(parameterizedImplementation, serviceType, entity));
            }
        }

        return (contexts, errors);
    }
    
}