using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Api;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

internal sealed class SideEffectActionStep<TEntity, TArguments>(
    Func<TArguments, SimulationStepContext<TEntity, TArguments>> contextProvider,
    Action<SimulationStepContext<TEntity, TArguments>> sideEffect) : ISimulationStep
{
    public void Execute(TArguments args)
    {
        var context = contextProvider.Invoke(args);
        sideEffect.Invoke(context);
    }
}

internal sealed class AsyncSideEffectActionStep<TEntity, TArguments>(
    Func<TArguments, SimulationStepContext<TEntity, TArguments>> contextProvider,
    Func<SimulationStepContext<TEntity, TArguments>, ValueTask> sideEffect) : ISimulationStep
{
    public async Task Execute(TArguments args)
    {
        var context = contextProvider.Invoke(args);
        await sideEffect.Invoke(context);
    }
}