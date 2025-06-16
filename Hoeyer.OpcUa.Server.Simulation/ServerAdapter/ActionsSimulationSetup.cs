using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Builder;
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter;

internal sealed class ActionSimulationSetup<TEntity, TMethodArgs>(
    IActionSimulationExecutor<TMethodArgs> executor,
    IEnumerable<IActionSimulationConfigurator<TEntity, TMethodArgs>> simulators,
    IEntityMethodArgTranslator<TMethodArgs> argsMapper,
    IEntityTranslator<TEntity> entityTranslator)
    : IPreinitializedNodeConfigurator<TEntity>
{
    public void Configure(IEntityNode node)
    {
        IOpcMethodArgumentsAttribute? annotation =
            typeof(TMethodArgs).GetCustomAttributes().OfType<IOpcMethodArgumentsAttribute>().First();
        AssertConfigurators(node, annotation);

        IActionSimulationConfigurator<TEntity, TMethodArgs> simulator = simulators.First();
        var builder = new ActionSimulationBuilder<TEntity, TMethodArgs>(node,
            new SimulationStepFactory<TEntity, TMethodArgs>(entityTranslator));
        MethodState method = node.Methods.First(e => e.BrowseName.Name.Equals(annotation.MethodName));
        IEnumerable<ISimulationStep> simultationSteps = simulator.ConfigureSimulation(builder);

        method.OnCallMethod += (context,
            methodState,
            inputArguments,
            outputArguments) =>
        {
            TMethodArgs? argumentStructure = argsMapper.Map(inputArguments);
            if (Equals(argumentStructure, default(TMethodArgs)))
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument,
                    new SimulationFailureException(
                        $"The method {method.BrowseName.Name} was called with invalid variables and could not be processed."));
            }

            try
            {
                executor.ExecuteSimulation(simultationSteps, argumentStructure!);
            }
            catch (Exception e)
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument,
                    new SimulationFailureException("The simulation execution failed: " + e));
            }

            return StatusCodes.Good;
        };
    }

    private void AssertConfigurators(IEntityNode node, IOpcMethodArgumentsAttribute annotation)
    {
        if (annotation is null)
        {
            throw new SimulationConfigurationException(
                $"The type '{typeof(TMethodArgs).FullName}' must be annotated with {nameof(IOpcMethodArgumentsAttribute)}. If you are creating a simulation for a method from an interface annotated with {nameof(OpcUaEntityMethodsAttribute<object>)}, then use the generated method args: XXXArgs.");
        }

        if (simulators.Count() > 1)
        {
            throw new SimulationConfigurationException(
                $"Multiple simulators where found for {annotation.Interface.FullName}.{annotation.MethodName}");
        }

        MethodState[] methods = node.Methods.Where(e => e.BrowseName.Name == annotation.MethodName).ToArray();
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