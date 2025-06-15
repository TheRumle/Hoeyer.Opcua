using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Action;

public interface IActionSimulationBuilder<TEntity, TArguments>
{
    IActionSimulationBuilder<TEntity, TArguments> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange);

    IActionSimulationBuilder<TEntity, TArguments> Wait(TimeSpan timeSpan);
    IEnumerable<ISimulationStep> Build();
}