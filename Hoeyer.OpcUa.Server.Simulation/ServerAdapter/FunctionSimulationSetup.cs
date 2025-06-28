using System;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Builder;
using Hoeyer.OpcUa.Server.Simulation.Services.Function;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter;

internal sealed class FunctionSimulationSetup<TEntity, TMethodArgs, TReturnType>(
    IFunctionSimulationExecutor<TEntity, TMethodArgs, TReturnType> executor,
    IFunctionSimulationConfigurator<TEntity, TMethodArgs, TReturnType> configurator,
    IEntityMethodArgTranslator<TMethodArgs> argsMapper,
    ISimulationStepFactory<TEntity, TMethodArgs> simulationStepFactory) : IPreinitializedNodeConfigurator<TEntity>
{
    private readonly int _numberOfArgs = typeof(TMethodArgs).GetProperties().Length;


    public void Configure(IManagedEntityNode managed)
    {
        var annotation = typeof(TMethodArgs).GetCustomAttributes().OfType<IOpcMethodArgumentsAttribute>().First();
        managed.Examine(n => AssertConfigurators(n, annotation));

        var builder = new FunctionSimulationBuilder<TEntity, TMethodArgs, TReturnType>(managed, simulationStepFactory);
        var simulationSteps = configurator.ConfigureSimulation(builder);

        var method =
            managed.Select(e => e.Methods.First(method => method.BrowseName.Name.Equals(annotation.MethodName)));
        method.OnCallMethod += (context,
            methodState,
            inputArguments,
            outputArguments) =>
        {
            try
            {
                TMethodArgs? argumentStructure = argsMapper.Map(inputArguments);
                if (inputArguments.Count != _numberOfArgs)
                {
                    return new ServiceResult(StatusCodes.BadInvalidArgument,
                        new SimulationFailureException(
                            $"The method {method.BrowseName.Name} was called with invalid number variables and could not be processed."));
                }

                var executionResult = executor.ExecuteSimulation(simulationSteps, argumentStructure!, context).Result;
                outputArguments[0] = executionResult.ReturnValue;
                return StatusCodes.Good;
            }
            catch (SimulationFailureException configurationException)
            {
                return new ServiceResult(StatusCodes.BadConfigurationError,
                    new SimulationFailureException(configurationException.Message));
            }
            catch (Exception e)
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument,
                    new SimulationFailureException("The simulation execution failed: " + e));
            }
        };
    }

    private static void AssertConfigurators(IEntityNode node, IOpcMethodArgumentsAttribute annotation)
    {
        if (annotation is null)
        {
            throw new SimulationConfigurationException(
                $"The type '{typeof(TMethodArgs).FullName}' must be annotated with {nameof(IOpcMethodArgumentsAttribute)}. If you are creating a simulation for a method from an interface annotated with {nameof(OpcUaEntityMethodsAttribute<object>)}, then use the generated method args: XXXArgs.");
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