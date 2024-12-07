using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;

namespace Hoeyer.Machines.OpcUa.Configuration.Services;

internal static class OpcUaServiceTypesExtensions
{
    public static readonly Type ConfiguratorType = typeof(IOpcUaNodeConfiguration<>);
    public static IEnumerable<OpcUaNodeConfigurationType> OpcUaConfigurators(this Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(type => IsConcreteAssignableFrom(type, ConfiguratorType))
            .Select(type => new OpcUaNodeConfigurationType(type, type.GetNodeParameter()));
    }
    
    public static bool IsConcreteAssignableFrom(this Type type, Type shouldBeAssignableTo)
    {
        return type.IsClass && !type.IsAbstract && shouldBeAssignableTo.IsAssignableFrom(type);
    }
    private static Type GetNodeParameter(this Type type)
    {
        return Array.Find(type.GetInterfaces(), static i => i.IsGenericType && i.GetGenericTypeDefinition() == ConfiguratorType) 
               ?? throw new OpcuaConfigurationException($"Type '{type.FullName}' does not implement '{ConfiguratorType.FullName}' correctly. It does not have a generic argument.");
    }
}