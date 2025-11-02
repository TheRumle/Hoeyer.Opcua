using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;

public class SimulationConfigurationException(string message) : Exception(message)
{
    public SimulationConfigurationException(IEnumerable<SimulationConfigurationException> otherExceptions)
        : this("Exceptions were encountered when trying to configure simulation framework: \n\n" +
               string.Join(",", otherExceptions.Select(e => e.Message)))
    {
    }
}