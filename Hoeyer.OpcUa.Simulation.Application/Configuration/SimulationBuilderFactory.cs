using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;

namespace Hoeyer.OpcUa.Simulation.Configuration;

public class SimulationBuilderFactory<TState, TArgs>(IEntityTranslator<TState> translator)
    : ISimulationBuilderFactory<TState, TArgs>
{
    public ISimulationBuilder<TState, TArgs> CreateSimulationBuilder() =>
        new SimulationBuilder<TState, TArgs>(translator);
}

public class SimulationBuilderFactory<TState, TArgs, TReturn>(IEntityTranslator<TState> translator)
    : ISimulationBuilderFactory<TState, TArgs, TReturn>
{
    public ISimulationBuilder<TState, TArgs, TReturn> CreateSimulationBuilder() =>
        new SimulationBuilder<TState, TArgs, TReturn>(translator);
}