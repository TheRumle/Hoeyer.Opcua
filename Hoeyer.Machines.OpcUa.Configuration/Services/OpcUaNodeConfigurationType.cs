using System;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;

namespace Hoeyer.Machines.OpcUa.Configuration.Services;

internal record struct OpcUaNodeConfigurationType
{
    public readonly Type ImplementorType;
    public readonly Type NodeType;

    public OpcUaNodeConfigurationType(Type implementorType, Type nodeType)
    {
        if (!implementorType.IsConcreteAssignableFrom(nodeType))
            throw new ArgumentException($"The type {nodeType} is not a concrete type of {ConfiguratorType.Name}<{nodeType.Name}>");
        
        ImplementorType = implementorType;
        NodeType = nodeType;
        GenericInterfaceType = ConfiguratorType.MakeGenericType(NodeType);
        NodeSectionSelectionStepType = typeof(NodeSectionSelectionStep<>).MakeGenericType(NodeType);
        ConfigureMethod = ImplementorType.GetMethod("Configure")!;
        NodeSelectionTypeInstance =(INodeSectionSelectionStep) Activator.CreateInstance(NodeSectionSelectionStepType);
    }

    public INodeSectionSelectionStep NodeSelectionTypeInstance { get; }

    public readonly MethodInfo ConfigureMethod;
    public readonly Type NodeSectionSelectionStepType;
    public readonly Type GenericInterfaceType;
    public static readonly Type ConfiguratorType = typeof(IOpcUaNodeConfiguration<>);
    
    public void InvokeConfigureMethod()
    {
        ConfigureMethod.Invoke(NodeSelectionTypeInstance, []);
    }
}