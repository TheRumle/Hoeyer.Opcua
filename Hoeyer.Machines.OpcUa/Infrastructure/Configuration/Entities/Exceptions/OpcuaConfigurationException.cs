using System;
using System.Collections.Generic;

namespace Hoeyer.Machines.OpcUa.Entities.Exceptions;

public class OpcuaConfigurationException(string message) : Exception(message)
{

    public static OpcuaConfigurationException Merge(IEnumerable<OpcuaConfigurationException> exceptions)
    {
        return new OpcuaConfigurationException(string.Join(Environment.NewLine, exceptions));
    }
    
}