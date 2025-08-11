using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

public sealed class AdaptionContextTranslator<TAgent, TMethodArgs>(
    IAgentTranslator<TAgent> agentTranslator,
    ISimulationBuilderFactory<TAgent, TMethodArgs> simulationBuilderFactory,
    ISimulation<TAgent, TMethodArgs> simulator,
    IMethodArgumentParser<TMethodArgs> argsParser
) : IAdaptionContextTranslator<(IList<object>, IManagedAgent), TAgent, TMethodArgs>
{
    public (TAgent currentState, TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps)
        CreateSimulationContext((IList<object>, IManagedAgent) context)
    {
        var (inputArguments, managedAgent) = context;
        var builder = simulationBuilderFactory.CreateSimulationBuilder();
        var simulationSteps = simulator.ConfigureSimulation(builder);
        var args = argsParser.ParseToArgsStructure(inputArguments);
        var currentState = managedAgent.Select(agentTranslator.Translate);
        return (currentState, args, simulationSteps);
    }
}

public sealed class AdaptionContextTranslator<TAgent, TMethodArgs, TReturn>(
    IAgentTranslator<TAgent> agentTranslator,
    ISimulationBuilderFactory<TAgent, TMethodArgs, TReturn> simulationBuilderFactory,
    ISimulation<TAgent, TMethodArgs, TReturn> simulator,
    IMethodArgumentParser<TMethodArgs> argsParser
) : IAdaptionContextTranslator<(IList<object>, IManagedAgent), TAgent, TMethodArgs, TReturn>
{
    public (TAgent currentState, TMethodArgs args, IEnumerable<ISimulationStep> simulationSteps)
        CreateSimulationContext((IList<object>, IManagedAgent) context)
    {
        var (inputArguments, managedAgent) = context;
        var builder = simulationBuilderFactory.CreateSimulationBuilder();
        var simulationSteps = simulator.ConfigureSimulation(builder);
        var args = argsParser.ParseToArgsStructure(inputArguments);
        var currentState = managedAgent.Select(agentTranslator.Translate);
        return (currentState, args, simulationSteps);
    }
}