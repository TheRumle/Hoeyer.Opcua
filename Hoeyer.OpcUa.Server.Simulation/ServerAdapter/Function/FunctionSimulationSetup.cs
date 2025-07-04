using System;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Action;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Function;

internal sealed class FunctionSimulationSetup<TEntity, TMethodArgs, TReturnType>(
    IFunctionSimulationConfigurator<TEntity, TMethodArgs, TReturnType> simulator,
    IFunctionSimulationOrchestrator<TReturnType> orchestrator,
    IFunctionSimulationBuilderFactory<TEntity, TMethodArgs, TReturnType> simulationBuilderFactory,
    ISimulationExecutorErrorHandler errorHandler,
    IOpcMethodArgumentsAttributeUsageValidator argsTypeAnnotationValidator) : IPreinitializedNodeConfigurator<TEntity>
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
                var executionResult = orchestrator.ExecuteMethodSimulation(inputArguments, simultationSteps, context)
                    .Result;
                outputArguments[0] = executionResult;
                return StatusCodes.Good;
            }
            catch (Exception e)
            {
                return errorHandler.HandleError(e);
            }
        };
    }
}