using System;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public interface ISimulationBuilder<TEntity, TArguments, out TBuilder>
{
    TBuilder ChangeState(Action<SimulationStepContext<TEntity, TArguments>> stateChange);
    TBuilder ChangeStateAsync(Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange);
    TBuilder SideEffect(Action<SimulationStepContext<TEntity, TArguments>> sideEffect);
    TBuilder Wait(TimeSpan timeSpan);
}