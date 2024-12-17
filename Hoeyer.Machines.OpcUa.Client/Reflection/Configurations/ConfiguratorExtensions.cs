using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Machines.OpcUa.Client.Infrastructure.Configuration.Entities.Builder;
using Hoeyer.Machines.OpcUa.Client.Infrastructure.Configuration.Entities.Exceptions;
using Hoeyer.Machines.OpcUa.Client.Reflection.Types;

namespace Hoeyer.Machines.OpcUa.Client.Reflection.Configurations;

internal static class ConfiguratorExtensions
{
    private static readonly Type ConfiguratorType = typeof(IOpcEntityConfigurator<>);
    public static IEnumerable<EntityConfigurationContext> GetEntityConfigContexts(this IEnumerable<Type> assembly)
    {
        return assembly
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(type => type.ImplementsGenericInterfaceOf(ConfiguratorType))
            .Select(type =>
            {
                var node = type.GetNodeParameter();
                return new EntityConfigurationContext(type, ConfiguratorType.MakeGenericType(node), node);
            });
    }

    private static Type GetNodeParameter(this Type type)
    {
        var nodeConfiguratorInterface =  Array.Find(type.GetInterfaces(), static i => i.IsGenericType && i.GetGenericTypeDefinition() == ConfiguratorType);
            
        return nodeConfiguratorInterface.GenericTypeArguments.FirstOrDefault() 
               ?? throw new OpcuaConfigurationException($"Type '{type.FullName}' does not implement '{ConfiguratorType.FullName}' correctly. It does not have a generic argument.");
    }
}