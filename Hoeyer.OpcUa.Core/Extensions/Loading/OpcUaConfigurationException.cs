using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Extensions.Loading;

public class OpcUaConfigurationException(string message) : Exception(message)
{
    public OpcUaConfigurationException(IEnumerable<OpcUaConfigurationException> configurationExceptions) : this(
        string.Join("\n", configurationExceptions))
    {
    }

    public static OpcUaConfigurationException ServiceNotConfigured(Type entity, Type service)
    {
        var missingService = service.IsGenericType ? service.GetGenericTypeDefinition().Name : service.Name;
        return new OpcUaConfigurationException(
            $"The entity {entity.Name} does not have any {missingService} configured!");
    }
}