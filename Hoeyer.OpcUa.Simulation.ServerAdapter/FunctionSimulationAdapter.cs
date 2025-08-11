using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

internal sealed class FunctionSimulationAdapter<TAgent, TMethodArgs, TReturnType>(
    IAdaptionContextTranslator<(IList<object>, IManagedAgent), TAgent, TMethodArgs, TReturnType>
        contextTranslator,
    ISimulationOrchestrator<TAgent, TMethodArgs, TReturnType> orchestrator,
    ISimulationExecutorErrorHandler errorHandler,
    IOpcMethodArgumentsAttributeUsageValidator argsTypeAnnotationValidator)
    : INodeConfigurator<TAgent>
{
    public void Configure(IManagedAgent managed, ISystemContext context)
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