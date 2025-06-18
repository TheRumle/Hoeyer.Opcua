using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Action;

internal class ActionSimulationExecutor<TEntity, TArgs>(ITimeScaler scaler) : IActionSimulationExecutor<TArgs>
{
    /// <inheritdoc />
    public virtual async ValueTask ExecuteSimulation(IEnumerable<ISimulationStep> steps, TArgs args)
    {
        foreach (ISimulationStep? step in steps)
        {
            switch (step)
            {
                case ActionStep<TEntity, TArgs> actionStep:
                    actionStep.Execute(args);
                    break;
                case TimeStep<TEntity> timeStep:
                    await Sleep(timeStep);
                    break;
                case SideEffectActionStep<TArgs> sideEffectStep:
                    sideEffectStep.Execute(args);
                    break;
                case AsyncActionStep<TEntity, TArgs> asyncActionStep:
                    await asyncActionStep.Execute(args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(step.GetType().Name +
                                                          " is not supported. Only IActionStep and ITimeStep are supported actions in the simulation framework");
            }
        }
    }

    private async Task Sleep(ITimeStep timeStep)
    {
        TimeSpan scaledTime = scaler.ScaleDown(timeStep.TimeSpan);
        await Task.Delay(scaledTime);
    }
}