using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities.Exceptions;
using Hoeyer.Machines.OpcUa.Reflection.Configurations;
using Hoeyer.Machines.OpcUa.Reflection.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Machines.OpcUa.Services;

public static class OpcUaServiceExtensions
{
    private static IReadOnlyList<EntityConfigurationContext> AddConfigurators(this IServiceCollection serviceCollection, IEnumerable<Type> types)
    {   
        var configurationContexts = types.GetEntityConfigContexts().ToList();
        ValidateContexts(configurationContexts);
        return configurationContexts;
    }

    private static void ValidateContexts(List<EntityConfigurationContext> configurationContexts)
    {
        OpcuaConfigurationException[] invalidContextExceptions =  configurationContexts
            .Where(context =>  !context.EntityType.HasEmptyConstructor())
            .Select(context=> new OpcuaConfigurationException($"{context.EntityType.Name} must have an empty constructor to be configured."))
            .ToArray();

        if (invalidContextExceptions.Any())
        {
            throw OpcuaConfigurationException.Merge(invalidContextExceptions);
        }
    }

    public static IServiceCollection AddOpcUa(this IServiceCollection services)
    {
        //Find and add all configurators to service collection and
        //build a service provider to resolve configurator dependencies s.t they can be used.
        var configurators = services.AddConfigurators(Assembly.GetCallingAssembly().GetTypes());
        using (var serviceConfigurator = new ServiceRegistry(services))
        {
            serviceConfigurator.ConfigureServices(configurators);
            return services;
        }
    }
}