using System.Collections.Generic;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

public interface IAdaptionContextTranslator<TContext, TAgent, TMethodArgs>
{
    public (TAgent currentState, TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps)
        CreateSimulationContext(TContext cont);
}

public interface IAdaptionContextTranslator<in TContext, TAgent, TMethodArgs, TReturn>
{
    public (TAgent currentState, TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps)
        CreateSimulationContext(TContext context);
}