using System;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public sealed class SimulationFailureException(string reason)
    : Exception("The simulation has halted for the following reason: " + reason);