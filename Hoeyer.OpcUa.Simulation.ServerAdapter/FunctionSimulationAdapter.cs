using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

internal sealed class FunctionSimulationAdapter<TEntity, TMethodArgs, TReturnType>(
    IAdaptionContextTranslator<(IList<object>, IManagedEntityNode), TEntity, TMethodArgs, TReturnType>
        contextTranslator,
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
                var (initialState, args, simulationSteps) =
                    contextTranslator.CreateSimulationContext((inputArguments, managed));
                var result = orchestrator.ExecuteMethodSimulation(initialState, args, simulationSteps).Result;
                outputArguments[0] = result;
                return StatusCodes.Good;
            }
            catch (Exception e)
            {
                return errorHandler.HandleError(e, method);
            }
        };
    }
}