using System.Collections.Generic;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;

namespace Hoeyer.OpcUa.Simulation.PostProcessing;

internal sealed class PipelineInitializer<TState, TProcessor>(
    ISubscriptionManager<SimulationResult<TState>> stepManager,
    IEnumerable<TProcessor> processors)
    where TProcessor : IStateChangeSimulationProcessor<TState>
{
    public ISubscriptionManager<SimulationResult<TState>> InitializeSimulationPipeline()
    {
        foreach (var stepProcessor in processors)
        {
            IMessageSubscription<SimulationResult<TState>> subscription = stepManager.Subscribe(stepProcessor);
            stepProcessor.AssignContext(new SimulationExecutionContext(subscription));
        }

        return stepManager;
    }
}