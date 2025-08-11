using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Simulation.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation;

public sealed class OpcMethodArgumentsAttributeUsageValidator : IOpcMethodArgumentsAttributeUsageValidator
{
    public MethodState ValidateAndGetMethodState<TMethodArgs>(IAgent managed) =>
        GetOrThrow<TMethodArgs>(managed);

    private static MethodState GetOrThrow<TMethodArgs>(IAgent managed)
    {
        var argsName = typeof(TMethodArgs).Name;
        var attrName = nameof(IOpcMethodArgumentsAttribute);
        var annotation = typeof(TMethodArgs).GetCustomAttributes().OfType<IOpcMethodArgumentsAttribute>()
            .FirstOrDefault();
        if (annotation == null)
        {
            throw new SimulationConfigurationException(
                $"The arguments type {argsName} is not annotated with any {attrName}");
        }

        AssertConfigurators(managed, annotation, argsName);
        var method = managed.Methods.FirstOrDefault(method => method.BrowseName.Name.Equals(annotation.MethodName));
        if (method == null)
        {
            throw new SimulationConfigurationException(
                $"The arguments type {argsName} does not point to a method on the entity {managed.BaseObject.BrowseName.Name}");
        }

        return method;
    }

    private static void AssertConfigurators(IAgent node, IOpcMethodArgumentsAttribute annotation, string argsName)
    {
        if (annotation is null)
        {
            throw new SimulationConfigurationException(
                $"The type '{argsName}' must be annotated with {nameof(IOpcMethodArgumentsAttribute)}. If you are creating a simulation for a method from an interface annotated with {nameof(OpcUaEntityMethodsAttribute<object>)}, then use the generated method args: XXXArgs.");
        }

        var methods = node.Methods.Where(e => e.BrowseName.Name == annotation.MethodName).ToArray();
        if (methods.Length > 1)
        {
            throw new SimulationConfigurationException(
                $"Multiple methods matched BrowseName {annotation.MethodName} on entity {annotation.Entity.Name}");
        }

        if (methods.Length == 0)
        {
            throw new SimulationConfigurationException(
                $"No method matched BrowseName {annotation.MethodName} on entity {annotation.Entity.Name}");
        }
    }
}