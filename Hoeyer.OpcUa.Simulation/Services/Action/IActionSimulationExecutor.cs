using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Action;

internal interface IActionSimulationExecutor<TEntity, in TArgs>
{
    IAsyncEnumerable<(TEntity Previous, DateTime Time, TEntity Reached)> ExecuteSimulation(
        IEnumerable<ISimulationStep> steps, TArgs args, ISystemContext systemContext);
}