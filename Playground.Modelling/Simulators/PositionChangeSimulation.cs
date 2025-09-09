﻿using Hoeyer.OpcUa.EntityModelling.Methods.Generated;
using Hoeyer.OpcUa.EntityModelling.Models;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.EntityModelling.Simulators;

public sealed class PositionChangeSimulation(ILogger<PositionChangeSimulation> logger)
    : ISimulation<Gantry, ChangePositionArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(ISimulationBuilder<Gantry, ChangePositionArgs> onMethodCall)
    {
        return onMethodCall
            .SideEffect(args => logger.LogDebug("Changing position to {Position}", args.Arguments.Position))
            .Wait(TimeSpan.FromSeconds(1))
            .ChangeState(e => e.State.Position = e.Arguments.Position)
            .Build();
    }
}

public sealed class ContainerSimulation : ISimulation<Gantry, PlaceContainerArgs, int>,
    ISimulation<Gantry, AssignContainerArgs>
{
    private readonly Random _random = new();

    /// <inheritdoc />
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<Gantry, AssignContainerArgs> onMethodCall)
    {
        return onMethodCall
            .ChangeState(e => e.State.HeldContainer = e.Arguments.Guid)
            .Build();
    }

    public IEnumerable<ISimulationStep> ConfigureSimulation(ISimulationBuilder<Gantry, PlaceContainerArgs, int> config)
    {
        return config.WithReturnValue(state => _random.Next());
    }
}