using System;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public sealed class SimulationFailureException(string reason)
    : Exception("The simulation failed while being executed runtime: " + reason);