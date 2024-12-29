using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hoeyer.OpcUa.Client.Configuration.Entities.Exceptions;

public class OpcuaConfigurationException(string message) : Exception(message)
{

    [Pure]
    public static OpcuaConfigurationException Merge(IEnumerable<OpcuaConfigurationException> exceptions)
    {
        return new OpcuaConfigurationException(string.Join(Environment.NewLine, exceptions));
    }
    
}