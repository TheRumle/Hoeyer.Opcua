using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public class OpcUaEntityServiceConfigurationException(string message) : Exception(message)
{
    public OpcUaEntityServiceConfigurationException(
        IEnumerable<Exception> configurationExceptions) :
        this(string.Join("\n", configurationExceptions.Select(e => e.Message)))
    {
    }
}