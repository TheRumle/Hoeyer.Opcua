using System;
using System.Linq;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration;
using Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration.Builder;
using Hoeyer.Machines.OpcUa.Configuration.Reflection.Types;

namespace Hoeyer.Machines.OpcUa.Configuration.Reflection.Configurations;

internal readonly record struct EntityConfigurationContext
{
    public readonly Type ImplementorType;
    public readonly Type ConcreteConfiguratorInterface;
    public readonly Type EntityType;
  
    public EntityConfigurationContext(Type implementorType, Type concreteConfiguratorInterface, Type entityType)
    {
        if (!concreteConfiguratorInterface.HasGenericParameterOfType(entityType))
            throw new ArgumentException($"The type {entityType} is not a concrete type of {concreteConfiguratorInterface.Name}<{entityType.Name}>");

        if (Array.Find(implementorType.GetInterfaces(), i => i == concreteConfiguratorInterface) == null)
        {
            throw new ArgumentException($"{implementorType.FullName} does not implement {concreteConfiguratorInterface.Name}<{entityType.Name}>");
        }
        
        ImplementorType = implementorType;
        ConcreteConfiguratorInterface = concreteConfiguratorInterface;
        EntityType = entityType;
    }

    /// <summary>
    /// Tries to invoke the "Configure" method of the Implementor type. First, the <see cref="EntityConfigurationBuilder{TNodeType}"/> of  is constructed through reflection, and then the <see cref="IOpcEntityConfigurator{T}.Configure"/> method is invoked by using an Activator and the provided <paramref name="implementorInstance"/>.
    /// </summary>
    /// <param name="implementorInstance">an instance of <see cref="EntityConfigurationContext.ImplementorType"/></param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="implementorInstance"/> is not of type <see cref="ImplementorType"/>.
    /// </exception>
    public EntityConfiguration<TEntity> InvokeConfigurationMethod<TEntity>(object implementorInstance) where TEntity : new()
    {
        if(implementorInstance.GetType() != ImplementorType) throw new ArgumentException("ImplementorType must be of type " + ImplementorType);

        var configureMethod = ImplementorType
            .GetMethods()
            .First(e => e.IsPublic && e.ReturnType == typeof(void) && MatchesConfigureMethodName(e));

        var builder = new EntityConfigurationBuilder<TEntity>();
        configureMethod.Invoke(implementorInstance, [builder]); 
        return builder.Context;
    }

    private static bool MatchesConfigureMethodName(MethodInfo e)
    {
        return nameof(IOpcEntityConfigurator<object>.Configure).Equals(e.Name, StringComparison.InvariantCulture);
    }
}