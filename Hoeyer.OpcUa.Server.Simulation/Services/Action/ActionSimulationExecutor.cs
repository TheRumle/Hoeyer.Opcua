using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Action;

internal class ActionSimulationExecutor<TArgs>(ITimeScaler scaler) : IActionSimulationExecutor<TArgs>
{
    /// <inheritdoc />
    public virtual async ValueTask ExecuteSimulation(IEnumerable<ISimulationStep> steps, TArgs args)
    {
        foreach (ISimulationStep? step in steps)
        {
            switch (step)
            {
                case IActionStep<TArgs> actionStep:
                    actionStep.Execute(args);
                    break;
                case ITimeStep timeStep:
                    await Sleep(timeStep);
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