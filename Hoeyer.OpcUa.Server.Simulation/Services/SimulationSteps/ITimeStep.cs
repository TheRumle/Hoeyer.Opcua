using System;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

public interface ITimeStep : ISimulationStep
{
    internal TimeSpan TimeSpan { get; }
}