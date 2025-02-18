using System;
using Hoeyer.OpcUa.Client.Reflection.Types;

namespace Hoeyer.OpcUa.Client.Reflection.Configurations;

internal readonly record struct EntityConfigurationContext
{
    public readonly Type EntityConfiguratorInterface;
    public readonly Type EntityType;
    public readonly Type EntityConfiguratorImpl;

    public EntityConfigurationContext(Type entityConfiguratorImpl, Type entityConfiguratorInterface, Type entityType)
    {
        if (!entityConfiguratorInterface.HasGenericParameterOfType(entityType))
            throw new ArgumentException(
                $"The type {entityType} is not a concrete type of {entityConfiguratorInterface.Name}<{entityType.Name}>");

        if (Array.Find(entityConfiguratorImpl.GetInterfaces(), i => i == entityConfiguratorInterface) == null)
            throw new ArgumentException(
                $"{entityConfiguratorImpl.FullName} does not implement {entityConfiguratorInterface.Name}<{entityType.Name}>");

        EntityConfiguratorImpl = entityConfiguratorImpl;
        EntityConfiguratorInterface = entityConfiguratorInterface;
        EntityType = entityType;
    }
}