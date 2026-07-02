using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Simulation.Abstractions.Execution;

namespace Hoeyer.OpcUa.Simulation.Abstractions.PostProcessing;

public interface IStateChangeSimulationProcessor<TState>
    : IMessageConsumer<SimulationResult<TState>>
{
    public void AssignContext(SimulationExecutionContext context);
}