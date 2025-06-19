using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public class SimulationConfigurationException(string message) : Exception(message)
{
    public SimulationConfigurationException(IEnumerable<SimulationConfigurationException> otherExceptions)
        : this("Exceptions were encountered when trying to configure simulation framework: \n\n" +
               string.Join("\n\n", otherExceptions.Select(e => e.Message)))
    {
    }
}