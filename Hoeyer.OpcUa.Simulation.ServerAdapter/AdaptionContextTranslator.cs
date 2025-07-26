using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

public sealed class AdaptionContextTranslator<TEntity, TMethodArgs>(
    IEntityTranslator<TEntity> entityTranslator,
    ISimulationBuilderFactory<TEntity, TMethodArgs> simulationBuilderFactory,
    ISimulation<TEntity, TMethodArgs> simulator,
    IMethodArgumentParser<TMethodArgs> argsParser
) : IAdaptionContextTranslator<(IList<object>, IManagedEntityNode), TEntity, TMethodArgs>
{
    public (TEntity currentState, TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps)
        CreateSimulationContext((IList<object>, IManagedEntityNode) context)
    {
        var (inputArguments, managedEntity) = context;
        var builder = simulationBuilderFactory.CreateSimulationBuilder();
        var simulationSteps = simulator.ConfigureSimulation(builder);
        var args = argsParser.ParseToArgsStructure(inputArguments);
        var currentState = managedEntity.Select(entityTranslator.Translate);
        return (currentState, args, simulationSteps);
    }
}

public sealed class AdaptionContextTranslator<TEntity, TMethodArgs, TReturn>(
    IEntityTranslator<TEntity> entityTranslator,
    ISimulationBuilderFactory<TEntity, TMethodArgs, TReturn> simulationBuilderFactory,
    ISimulation<TEntity, TMethodArgs, TReturn> simulator,
    IMethodArgumentParser<TMethodArgs> argsParser
) : IAdaptionContextTranslator<(IList<object>, IManagedEntityNode), TEntity, TMethodArgs, TReturn>
{
    public (TEntity currentState, TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps)
        CreateSimulationContext((IList<object>, IManagedEntityNode) context)
    {
        var (inputArguments, managedEntity) = context;
        var builder = simulationBuilderFactory.CreateSimulationBuilder();
        var simulationSteps = simulator.ConfigureSimulation(builder);
        var args = argsParser.ParseToArgsStructure(inputArguments);
        var currentState = managedEntity.Select(entityTranslator.Translate);
        return (currentState, args, simulationSteps);
    }
}