namespace Hoeyer.OpcUa.Simulation.Abstractions.Configuration.Exceptions;

public sealed class DuplicateSimulatorConfigurationException(string m) : SimulationConfigurationException(m);