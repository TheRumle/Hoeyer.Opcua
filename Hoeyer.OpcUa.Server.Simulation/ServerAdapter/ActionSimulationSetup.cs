using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Builder;
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter;

internal sealed class ActionSimulationSetup<TEntity, TMethodArgs>(
    IActionSimulationExecutor<TEntity, TMethodArgs> executor,
    IEnumerable<IActionSimulationConfigurator<TEntity, TMethodArgs>> simulators,
    IEntityMethodArgTranslator<TMethodArgs> argsMapper,
    IEntityTranslator<TEntity> entityTranslator)
    : IPreinitializedNodeConfigurator<TEntity>
{
    private readonly int _numberOfArgs = typeof(TMethodArgs).GetProperties().Length;
    
    public void Configure(IManagedEntityNode managed)
    {
        var annotation = typeof(TMethodArgs).GetCustomAttributes().OfType<IOpcMethodArgumentsAttribute>().First();
        managed.Examine(n => AssertConfigurators(n, annotation));

        var method = managed.Select(e => e.Methods.First(method => method.BrowseName.Name.Equals(annotation.MethodName)));
        var simultationSteps = ConfigureSimulation(managed);


        method.OnCallMethod += (context,
            methodState,
            inputArguments,
            outputArguments) =>
        {
            var argumentStructure = argsMapper.Map(inputArguments);
            if (inputArguments.Count != _numberOfArgs)
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument,
                    new SimulationFailureException(
                        $"The method {method.BrowseName.Name} was called with invalid number variables and could not be processed."));
            }

            try
            {
                var res = executor.ExecuteSimulation(simultationSteps, argumentStructure!, context).Collect().Result;
                return StatusCodes.Good;
            }
            catch (Exception e)
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument,
                    new SimulationFailureException("The simulation execution failed: " + e));
            }
        };
    }

    private IEnumerable<ISimulationStep> ConfigureSimulation(IManagedEntityNode node)
    {
        try
        {
            var simulator = simulators.First();
            var builder = new ActionSimulationBuilder<TEntity, TMethodArgs>(node, new SimulationStepFactory<TEntity, TMethodArgs>(entityTranslator));
            var simultationSteps = simulator.ConfigureSimulation(builder);
            return simultationSteps;
        }
        catch (Exception e)
        {
            throw new SimulationConfigurationException(e.Message);
        }
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
            var message =
                $"Multiple simulators where found for {annotation.Interface.FullName}.{annotation.MethodName}: \n"
                + string.Join(", ", simulators.Select(e => e.GetType().Name));
            throw new SimulationConfigurationException(message);
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