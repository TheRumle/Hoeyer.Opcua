using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Application.Node.Entity;
using Hoeyer.OpcUa.Server.ServiceConfiguration.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

public static class ServiceExtensions
{
    public static OpcServerServiceCollection AddOpcUaEntityServer(this IServiceCollection services, Func<IEntityServerConfigurationBuilder, EntityServerConfiguration> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke(EntityServerConfigurationBuilder.Create());
        services.AddSingleton(entityServerConfiguration);
        services.AddSingleton(p =>
        {
            var loggerFactory = p.GetService<ILoggerFactory>()!;
            return new OpcUaEntityServerFactory(entityServerConfiguration, [], new EntityNodeManagerFactory(loggerFactory));
        });
        return new OpcServerServiceCollection(services);
    }
    
    public static void WithEntityNodeGeneration(this OpcServerServiceCollection services)
    {
        services.Collection.AddSingleton<OpcUaEntityServerFactory>(p =>
        {
            var loggerFactory = p.GetService<ILoggerFactory>()!;
            var configuration = p.GetService<EntityServerConfiguration>();
            if (configuration is null) throw new InvalidOperationException($"No {nameof(EntityServerConfiguration)} has been configured!");
            return new OpcUaEntityServerFactory(configuration, GetEntityNodeCreators(), new EntityNodeManagerFactory(loggerFactory));
        });
    }

    private static IEnumerable<IEntityNodeCreator> GetEntityNodeCreators()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IEntityNodeCreator).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Where(e => e.GetConstructor(Type.EmptyTypes) != null)
            .Select(Activator.CreateInstance)
            .Cast<IEntityNodeCreator>();
    }
}