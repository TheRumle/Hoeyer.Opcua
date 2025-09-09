using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

public interface IAdaptionContextTranslator<TEntity, TMethodArgs>
{
    public (TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps)
        CreateSimulationContext(IList<object> inputArguments, IManagedEntityNode entity);
}

public interface IAdaptionContextTranslator<TEntity, TMethodArgs, TReturn>
{
    public (TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps) CreateSimulationContext(
        IList<object> inputArguments, IManagedEntityNode entity);
}