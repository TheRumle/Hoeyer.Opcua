using System;
using Hoeyer.OpcUa.Client.Reflection.Types;

namespace Hoeyer.OpcUa.Client.Reflection.Configurations;

internal readonly record struct EntityConfigurationContext
{
    public readonly Type ConcreteConfiguratorInterface;
    public readonly Type EntityType;
    public readonly Type ImplementorType;

    public EntityConfigurationContext(Type implementorType, Type concreteConfiguratorInterface, Type entityType)
    {
        if (!concreteConfiguratorInterface.HasGenericParameterOfType(entityType))
            throw new ArgumentException(
                $"The type {entityType} is not a concrete type of {concreteConfiguratorInterface.Name}<{entityType.Name}>");

        if (Array.Find(implementorType.GetInterfaces(), i => i == concreteConfiguratorInterface) == null)
            throw new ArgumentException(
                $"{implementorType.FullName} does not implement {concreteConfiguratorInterface.Name}<{entityType.Name}>");

        ImplementorType = implementorType;
        ConcreteConfiguratorInterface = concreteConfiguratorInterface;
        EntityType = entityType;
    }
}