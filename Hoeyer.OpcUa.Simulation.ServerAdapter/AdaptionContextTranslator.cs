using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.Configuration;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

public sealed class AdaptionContextTranslator<TEntity, TMethodArgs>(
    IEntityTranslator<TEntity> translator,
    ISimulation<TEntity, TMethodArgs> simulator,
    IMethodArgumentParser<TMethodArgs> argsParser
) : IAdaptionContextTranslator<TEntity, TMethodArgs>
{
    public (TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps) CreateSimulationContext(
        IList<object> inputArguments, IManagedEntityNode entity)
    {
        var builder =
            new SimulationBuilder<TEntity, TMethodArgs>(translator, new LockedEntityState<TEntity>(entity, translator));
        var simulationSteps = simulator.ConfigureSimulation(builder);
        var args = argsParser.ParseToArgsStructure(inputArguments);
        return (args, simulationSteps);
    }
}

public sealed class AdaptionContextTranslator<TEntity, TMethodArgs, TReturn>(
    IEntityTranslator<TEntity> translator,
    ISimulation<TEntity, TMethodArgs, TReturn> simulator,
    IMethodArgumentParser<TMethodArgs> argsParser
) : IAdaptionContextTranslator<TEntity, TMethodArgs, TReturn>
{
    public (TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps) CreateSimulationContext(
        IList<object> inputArguments, IManagedEntityNode entity)
    {
        var builder =
            new SimulationBuilder<TEntity, TMethodArgs, TReturn>(translator,
                new LockedEntityState<TEntity>(entity, translator));
        var simulationSteps = simulator.ConfigureSimulation(builder);
        var args = argsParser.ParseToArgsStructure(inputArguments);
        return (args, simulationSteps);
    }
}