using System;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Action;

internal interface IOpcMethodArgumentsAttributeUsageValidator
{
    MethodState ValidateAndGetMethodState<TMethodArgs>(IManagedEntityNode managed);
}

internal sealed class OpcMethodArgumentsAttributeUsageValidator : IOpcMethodArgumentsAttributeUsageValidator
{
    public MethodState ValidateAndGetMethodState<TMethodArgs>(IManagedEntityNode managed) =>
        GetOrThrow<TMethodArgs>(managed);

    private static MethodState GetOrThrow<TMethodArgs>(IManagedEntityNode managed)
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

        managed.Examine(n => AssertConfigurators(n, annotation, argsName));
        var method = managed.Select(e =>
            e.Methods.FirstOrDefault(method => method.BrowseName.Name.Equals(annotation.MethodName)));
        if (method == null)
        {
            throw new SimulationConfigurationException(
                $"The arguments type {argsName} does not point to a method on the entity {managed.EntityName}");
        }

        return method;
    }

    private static void AssertConfigurators(IEntityNode node, IOpcMethodArgumentsAttribute annotation, string argsName)
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

internal sealed class ActionSimulationSetup<TEntity, TMethodArgs>(
    IActionSimulationConfigurator<TEntity, TMethodArgs> simulator,
    IActionSimulationOrchestrator orchestrator,
    IActionSimulationBuilderFactory<TEntity, TMethodArgs> simulationBuilderFactory,
    ISimulationExecutorErrorHandler errorHandler,
    IOpcMethodArgumentsAttributeUsageValidator argsTypeAnnotationValidator)
    : IPreinitializedNodeConfigurator<TEntity>
{
    public void Configure(IManagedEntityNode managed)
    {
        var method = argsTypeAnnotationValidator.ValidateAndGetMethodState<TMethodArgs>(managed);
        method.OnCallMethod += (context,
            methodState,
            inputArguments,
            outputArguments) =>
        {
            try
            {
                var builder = simulationBuilderFactory.CreateBuilder(managed);
                var simultationSteps = simulator.ConfigureSimulation(builder);
                orchestrator.ExecuteMethodSimulation(inputArguments, simultationSteps, context);
                return StatusCodes.Good;
            }
            catch (Exception e)
            {
                return errorHandler.HandleError(e);
            }
        };
    }
}