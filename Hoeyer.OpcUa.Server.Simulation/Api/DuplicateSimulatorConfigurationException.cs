namespace Hoeyer.OpcUa.Server.Simulation.Api;

public sealed class DuplicateSimulatorConfigurationException(string m) : SimulationConfigurationException(m);