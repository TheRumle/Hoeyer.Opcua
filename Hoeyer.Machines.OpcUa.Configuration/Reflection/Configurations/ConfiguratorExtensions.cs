using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Machines.OpcUa.Configuration.Entity;
using Hoeyer.Machines.OpcUa.Configuration.Entity.Exceptions;
using Hoeyer.Machines.OpcUa.Configuration.Reflection.Types;

namespace Hoeyer.Machines.OpcUa.Configuration.Reflection.Configurations;

internal static class ConfiguratorExtensions
{
    public static readonly Type ConfiguratorType = typeof(IOpcUaNodeConfiguration<>);
    public static IEnumerable<OpcUaNodeConfiguratorContext> GetOpcUaConfigurators(this IEnumerable<Type> assembly)
    {
        return assembly
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(type => type.ImplementsGenericInterfaceOf(ConfiguratorType))
            .Select(type =>
            {
                var node = type.GetNodeParameter();
                return new OpcUaNodeConfiguratorContext(type, ConfiguratorType.MakeGenericType(node), node);
            });
    }

    private static Type GetNodeParameter(this Type type)
    {
        var nodeConfiguratorInterface =  Array.Find(type.GetInterfaces(), static i => i.IsGenericType && i.GetGenericTypeDefinition() == ConfiguratorType);
            
        return nodeConfiguratorInterface.GenericTypeArguments.FirstOrDefault() 
               ?? throw new OpcuaConfigurationException($"Type '{type.FullName}' does not implement '{ConfiguratorType.FullName}' correctly. It does not have a generic argument.");
    }
}