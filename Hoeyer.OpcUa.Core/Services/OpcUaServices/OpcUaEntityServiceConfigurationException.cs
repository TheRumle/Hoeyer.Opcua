using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public class OpcUaEntityServiceConfigurationException(string message) : Exception(message)
{
    public OpcUaEntityServiceConfigurationException(
        IEnumerable<OpcUaEntityServiceConfigurationException> configurationExceptions) :
        this(
            string.Join("\n", configurationExceptions))
    {
    }

    public OpcUaEntityServiceConfigurationException(
        params OpcUaEntityServiceConfigurationException[] configurationExceptions) :
        this(string.Join("\n", configurationExceptions.ToList()))
    {
    }

    public static OpcUaEntityServiceConfigurationException ServiceNotConfigured(Type entity, Type service)
    {
        var missingService = service.IsGenericType ? service.GetGenericTypeDefinition().Name : service.Name;
        return new OpcUaEntityServiceConfigurationException(
            $"The Entity {entity.Name} does not have any {missingService} configured!");
    }

    public static OpcUaEntityServiceConfigurationException NoCustomImplementation(Type entity, string service)
    {
        return new OpcUaEntityServiceConfigurationException(
            $"Could not find any implementation of service '{service}' for Entity {entity.Name}. Consider creating a public implementation of the service and it will be configured automatically");
    }
}