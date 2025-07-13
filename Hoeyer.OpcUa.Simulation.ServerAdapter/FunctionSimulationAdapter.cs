using System;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Configuration;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

internal sealed class FunctionSimulationAdapter<TEntity, TMethodArgs, TReturnType>(
    ILogger logger,
    IEntityTranslator<TEntity> translator,
    IMethodArgumentParser<TMethodArgs> argsParser,
    ISimulation<TEntity, TMethodArgs, TReturnType> simulator,
    ISimulationOrchestrator<TEntity, TMethodArgs, TReturnType> orchestrator,
    ISimulationExecutorErrorHandler errorHandler,
    IOpcMethodArgumentsAttributeUsageValidator argsTypeAnnotationValidator)
    : INodeConfigurator<TEntity>
{
    public void Configure(IManagedEntityNode managed, ISystemContext context)
    {
        var method = managed.Select(argsTypeAnnotationValidator.ValidateAndGetMethodState<TMethodArgs>);
        method.OnCallMethod += (context,
            methodState,
            inputArguments,
            outputArguments) =>
        {
            try
            {
                var builder = new SimulationBuilder<TEntity, TMethodArgs, TReturnType>(translator);
                var simulationSteps = simulator.ConfigureSimulation(builder);
                var initialState = managed.Select(translator.Translate);
                var args = argsParser.ParseToArgsStructure(inputArguments);
                var result = orchestrator.ExecuteMethodSimulation(initialState, args, simulationSteps).Result;
                outputArguments[0] = result;
                return StatusCodes.Good;
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured why executing simulation");
                return errorHandler.HandleError(e);
            }
        };
    }
}