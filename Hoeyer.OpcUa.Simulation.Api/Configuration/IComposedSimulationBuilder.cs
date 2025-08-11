using System;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Simulation.Api.Configuration;

public interface IComposedSimulationBuilder<TAgent, TArguments, out TBuilder>
{
    TBuilder ChangeState(Action<SimulationStepContext<TAgent, TArguments>> stateChange);
    TBuilder ChangeStateAsync(Func<SimulationStepContext<TAgent, TArguments>, ValueTask> stateChange);
    TBuilder SideEffect(Action<SimulationStepContext<TAgent, TArguments>> sideEffect);
    TBuilder Wait(TimeSpan timeSpan);
}