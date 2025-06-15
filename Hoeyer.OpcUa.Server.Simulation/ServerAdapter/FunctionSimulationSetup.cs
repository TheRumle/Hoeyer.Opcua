using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Builder;
using Hoeyer.OpcUa.Server.Simulation.Services.Function;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter;

internal sealed class FunctionSimulationSetup<TEntity, TMethodArgs, TReturnType>(
    IFunctionSimulationExecutor<TMethodArgs, TReturnType> executor,
    IEnumerable<IFunctionSimulationConfigurator<TEntity, TMethodArgs>> simulators,
    IEntityMethodArgTranslator<TMethodArgs> argsMapper,
    IEntityTranslator<TEntity> entityMapper)
{
    public void Configure(IEntityNode node)
    {
        IOpcMethodArgumentsAttribute? annotation =
            typeof(TMethodArgs).GetCustomAttributes().OfType<IOpcMethodArgumentsAttribute>().First();
        AssertConfigurators(node, annotation);

        IFunctionSimulationConfigurator<TEntity, TMethodArgs> simulator = simulators.First();
        var builder = new FunctionSimulationBuilder<TEntity, TMethodArgs>(node,
            new SimulationStepFactory<TEntity, TMethodArgs>(entityMapper));
        MethodState method = node.Methods.First(e => e.BrowseName.Name.Equals(annotation.MethodName));
        IEnumerable<ISimulationStep> simultationSteps = simulator.ConfigureSimulation(builder);

        method.OnCallMethod += (context,
            methodState,
            inputArguments,
            outputArguments) =>
        {
            try
            {
                TMethodArgs? argumentStructure = argsMapper.Map(inputArguments);
                if (Equals(argumentStructure, default(TMethodArgs)))
                {
                    return new ServiceResult(StatusCodes.BadInvalidArgument,
                        new SimulationFailureException(
                            $"The method {method.BrowseName.Name} was called with invalid variables and could not be processed."));
                }

                ValueTask<TReturnType> executionResult =
                    executor.ExecuteSimulation(simultationSteps, argumentStructure!);
                outputArguments[0] = executionResult!.Result;
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