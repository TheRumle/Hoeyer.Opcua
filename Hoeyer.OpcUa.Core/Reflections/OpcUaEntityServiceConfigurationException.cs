using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Reflections;

public class OpcUaEntityServiceConfigurationException(string message) : Exception(message)
{
    public OpcUaEntityServiceConfigurationException(IEnumerable<OpcUaEntityServiceConfigurationException> configurationExceptions) :
        this(
            string.Join("\n", configurationExceptions))
    {
    }

    public static OpcUaEntityServiceConfigurationException ServiceNotConfigured(Type entity, Type service)
    {
        var missingService = service.IsGenericType ? service.GetGenericTypeDefinition().Name : service.Name;
        return new OpcUaEntityServiceConfigurationException(
            $"The entity {entity.Name} does not have any {missingService} configured!");
    }
}