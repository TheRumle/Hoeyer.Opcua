using System;

namespace Hoeyer.OpcUa.Simulation.Abstractions.Configuration.Exceptions;

public sealed class SimulationFailureException(string reason)
    : Exception("The simulation failed while being executed runtime: " + reason);