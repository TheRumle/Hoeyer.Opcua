using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Extensions.Loading;

public class OpcUaServiceConfigurationException(string message) : Exception(message)
{
    public OpcUaServiceConfigurationException(IEnumerable<OpcUaServiceConfigurationException> configurationExceptions) : this(
        string.Join("\n", configurationExceptions))
    {
    }

    public static OpcUaServiceConfigurationException ServiceNotConfigured(Type entity, Type service)
    {
        var missingService = service.IsGenericType ? service.GetGenericTypeDefinition().Name : service.Name;
        return new OpcUaServiceConfigurationException(
            $"The entity {entity.Name} does not have any {missingService} configured!");
    }
}