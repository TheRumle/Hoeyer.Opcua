using System;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public sealed class SimulationConfigurationException(string message) : Exception(message);