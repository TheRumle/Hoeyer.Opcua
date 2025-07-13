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

internal sealed class ActionSimulationAdapter<TEntity, TMethodArgs>(
    ILogger<ActionSimulationAdapter<TEntity, TMethodArgs>> logger,
    IEntityTranslator<TEntity> translator,
    IMethodArgumentParser<TMethodArgs> argsParser,
    ISimulation<TEntity, TMethodArgs> simulator,
    ISimulationOrchestrator<TEntity, TMethodArgs> orchestrator,
    ISimulationExecutorErrorHandler errorHandler,
    IOpcMethodArgumentsAttributeUsageValidator argsTypeAnnotationValidator)
    : INodeConfigurator<TEntity>
{
    public void Configure(IManagedEntityNode managed, ISystemContext context)
    {
        var method = managed.Select(node => argsTypeAnnotationValidator.ValidateAndGetMethodState<TMethodArgs>(node));
        method.OnCallMethod += (context,
            methodState,
            inputArguments,
            outputArguments) =>
        {
            try
            {
                SimulationBuilder<TEntity, TMethodArgs> builder =
                    new SimulationBuilder<TEntity, TMethodArgs>(translator);
                var simulationSteps = simulator.ConfigureSimulation(builder);
                var initialState = managed.Select(translator.Translate);
                var args = argsParser.ParseToArgsStructure(inputArguments);
                orchestrator.ExecuteMethodSimulation(initialState, args, simulationSteps).Wait();
                return StatusCodes.Good;
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured during simulation execution");
                return errorHandler.HandleError(e);
            }
        };
    }
}