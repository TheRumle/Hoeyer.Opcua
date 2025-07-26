using System.Collections.Generic;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

public interface IAdaptionContextTranslator<TContext, TEntity, TMethodArgs>
{
    public (TEntity currentState, TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps)
        CreateSimulationContext(TContext cont);
}

public interface IAdaptionContextTranslator<in TContext, TEntity, TMethodArgs, TReturn>
{
    public (TEntity currentState, TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps)
        CreateSimulationContext(TContext context);
}