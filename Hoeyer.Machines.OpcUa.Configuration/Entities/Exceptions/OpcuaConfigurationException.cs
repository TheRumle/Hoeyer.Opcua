using System;
using System.Collections.Generic;

namespace Hoeyer.Machines.OpcUa.Configuration.Entities.Exceptions;

public class OpcuaConfigurationException(string message) : Exception(message)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return message;
    }

    public static OpcuaConfigurationException Aggregate(IEnumerable<OpcuaConfigurationException> exceptions)
    {
        return new OpcuaConfigurationException(string.Join(Environment.NewLine, exceptions));
    }
    
}