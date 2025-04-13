using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;

namespace Hoeyer.OpcUa.Core.Reflections;

public class OpcUaEntityServiceConfigurationException(string message) : Exception(message)
{
    public OpcUaEntityServiceConfigurationException(IEnumerable<OpcUaEntityServiceConfigurationException> configurationExceptions) :
        this(
            string.Join("\n", configurationExceptions))
    {
    }
    
    public OpcUaEntityServiceConfigurationException(params OpcUaEntityServiceConfigurationException[] configurationExceptions) :
        this(string.Join("\n", configurationExceptions.ToList()))
    {
    }

    public static OpcUaEntityServiceConfigurationException ServiceNotConfigured(Type entity, Type service)
    {
        var missingService = service.IsGenericType ? service.GetGenericTypeDefinition().Name : service.Name;
        return new OpcUaEntityServiceConfigurationException(
            $"The entity {entity.Name} does not have any {missingService} configured!");
    }
    
    public static OpcUaEntityServiceConfigurationException NoCustomImplementation(Type entity, string service)
    {
        return new OpcUaEntityServiceConfigurationException(
            $"Could not find any implementation of service '{service}' for entity {entity.Name}. Consider creating a public implementation of the service and it will be configured automatically");
    }
}