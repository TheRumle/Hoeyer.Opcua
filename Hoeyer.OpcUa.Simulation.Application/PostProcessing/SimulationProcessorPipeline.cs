using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;

namespace Hoeyer.OpcUa.Simulation.PostProcessing;

internal class SimulationProcessorPipeline<TState, TArgs> : ISimulationProcessorPipeline<TState, TArgs>
{
    private readonly ISubscriptionManager<SimulationResult<TState>> _manager;

    public SimulationProcessorPipeline(ISubscriptionManager<SimulationResult<TState>> manager,
        IEnumerable<IStateChangeSimulationProcessor<TState>> processors)
    {
        _manager = manager;
        foreach (var stepProcessor in processors)
        {
            var subscription = _manager.Subscribe(stepProcessor);
            stepProcessor.AssignContext(new SimulationExecutionContext(subscription));
        }
    }


    public ValueTask OnSimulationBegin(TArgs args)
    {
        return default;
    }

    public ValueTask ProcessStateChange(SimulationResult<TState> stateChange)
    {
        _manager.Publish(stateChange);
        return default;
    }

    /// <inheritdoc />
    public ValueTask OnSimulationFinished() => default;
}

internal sealed class SimulationProcessorPipeline<TState, TArgs, TReturn>(
    ISubscriptionManager<SimulationResult<TState>> stepManager,
    IEnumerable<IStateChangeSimulationProcessor<TState>> processors)
    : SimulationProcessorPipeline<TState, TArgs>(stepManager, processors),
        ISimulationProcessorPipeline<TState, TArgs, TReturn>
{
    public ValueTask OnValueReturned(TReturn producedValue) => default;
}