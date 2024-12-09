using System;
using System.Linq;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.Entity;
using Hoeyer.Machines.OpcUa.Configuration.Entity.Context;
using Hoeyer.Machines.OpcUa.Configuration.Reflection.Types;

namespace Hoeyer.Machines.OpcUa.Configuration.Reflection.Configurations;

internal readonly record struct OpcUaNodeConfiguratorContext
{
    public readonly Type ImplementorType;
    public readonly Type ConcreteConfiguratorInterface;
    public readonly Type NodeType;
  
    public OpcUaNodeConfiguratorContext(Type implementorType, Type concreteConfiguratorInterface, Type nodeType)
    {
        if (!concreteConfiguratorInterface.HasGenericParameterOfType(nodeType))
            throw new ArgumentException($"The type {nodeType} is not a concrete type of {concreteConfiguratorInterface.Name}<{nodeType.Name}>");

        if (Array.Find(implementorType.GetInterfaces(), i => i == concreteConfiguratorInterface) == null)
        {
            throw new ArgumentException($"{implementorType.FullName} does not implement {concreteConfiguratorInterface.Name}<{nodeType.Name}>");
        }
        
        ImplementorType = implementorType;
        ConcreteConfiguratorInterface = concreteConfiguratorInterface;
        NodeType = nodeType;
    }

    /// <summary>
    /// Tries to invoke the "Configure" method of the Implementor type. First, the <see cref="RootIdentityBuilder{TNodeType}"/> of  is constructed through reflection, and then the <see cref="IOpcUaNodeConfiguration.Configure"/> method is invoked by using an Activator and the provided <paramref name="implementorInstance"/>.
    /// </summary>
    /// <param name="implementorInstance">an instance of <see cref="OpcUaNodeConfiguratorContext.ImplementorType"/></param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="implementorInstance"/> is not of type <see cref="ImplementorType"/>.
    /// </exception>
    public OpcUaEntityConfiguration InvokeConfigurationMethod(object implementorInstance)
    {
        if(implementorInstance.GetType() != ImplementorType) throw new ArgumentException("ImplementorType must be of type " + ImplementorType);

        var configureMethod = ImplementorType
            .GetMethods()
            .First(e => e.IsPublic && e.ReturnType == typeof(void) && MatchesConfigureMethodName(e));

        var stepGenericType = typeof(RootIdentityBuilder<>).MakeGenericType(NodeType);
        var nodeSelectionStep = (INodeConfigurationContextHolder)Activator.CreateInstance(
                stepGenericType,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                null,
                null
            );
        
        configureMethod.Invoke(implementorInstance, [nodeSelectionStep]); 

        return nodeSelectionStep.Context;
    }

    private static bool MatchesConfigureMethodName(MethodInfo e)
    {
        return nameof(IOpcUaNodeConfiguration<int>.Configure).Equals(e.Name, StringComparison.InvariantCulture);
    }
}