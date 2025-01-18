using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Client.Configuration.Entities.Exceptions;
using Hoeyer.OpcUa.Client.Reflection.Configurations;
using Hoeyer.OpcUa.Client.Reflection.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client.Services;

public static class OpcUaServiceExtensions
{
    private static IReadOnlyList<EntityConfigurationContext> GetConfigurators(IEnumerable<Type> types)
    {
        var configurationContexts = types.GetEntityConfigContexts().ToList();
        ValidateContexts(configurationContexts);
        return configurationContexts;
    }

    private static void ValidateContexts(List<EntityConfigurationContext> configurationContexts)
    {
        OpcuaConfigurationException[] invalidContextExceptions = configurationContexts
            .Where(context => !context.EntityType.HasEmptyConstructor())
            .Select(context =>
                new OpcuaConfigurationException(
                    $"{context.EntityType.Name} must have an empty constructor to be configured."))
            .ToArray();

        if (invalidContextExceptions.Any()) throw OpcuaConfigurationException.Merge(invalidContextExceptions);
    }

    public static IServiceCollection AddOpcUaClientServices(this IServiceCollection services)
    {
        //Find and add all configurators to service collection and
        //build a service provider to resolve configurator dependencies s.t they can be used.
        var configurators = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => GetConfigurators(assembly.GetTypes()));

        using (var serviceConfigurator = new ServiceRegistry(services))
        {
            serviceConfigurator.ConfigureServices(configurators);
            return services;
        }
    }
}