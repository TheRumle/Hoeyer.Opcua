using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Simulation.Api.Execution;

namespace Hoeyer.OpcUa.Simulation.Api.PostProcessing;

public interface IStateChangeSimulationProcessor<TState>
    : IMessageConsumer<SimulationResult<TState>>
{
    public void AssignContext(SimulationExecutionContext context);
}