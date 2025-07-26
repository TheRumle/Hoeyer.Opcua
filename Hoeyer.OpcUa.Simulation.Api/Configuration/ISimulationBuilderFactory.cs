namespace Hoeyer.OpcUa.Simulation.Api.Configuration;

public interface ISimulationBuilderFactory<TState, TArgs>
{
    public ISimulationBuilder<TState, TArgs> CreateSimulationBuilder();
}

public interface ISimulationBuilderFactory<TState, TArgs, in TOut>
{
    public ISimulationBuilder<TState, TArgs, TOut> CreateSimulationBuilder();
}