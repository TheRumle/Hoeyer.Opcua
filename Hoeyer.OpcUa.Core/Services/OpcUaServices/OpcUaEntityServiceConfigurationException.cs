using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public class OpcUaEntityServiceConfigurationException(string message) : Exception(message)
{
    public OpcUaEntityServiceConfigurationException(
        IEnumerable<OpcUaEntityServiceConfigurationException> configurationExceptions) :
        this(
            string.Join("\n", configurationExceptions.Select(e => e.Message)))
    {
    }

    public OpcUaEntityServiceConfigurationException(
        IEnumerable<Exception> configurationExceptions) :
        this(string.Join("\n", configurationExceptions.Select(e => e.Message)))
    {
    }


    public OpcUaEntityServiceConfigurationException(
        params OpcUaEntityServiceConfigurationException[] configurationExceptions) :
        this(string.Join("\n", configurationExceptions.Select(e => e.Message)))
    {
    }

    public static OpcUaEntityServiceConfigurationException ServiceNotConfigured(Type entity, Type service)
    {
        var missingService = service.IsGenericType ? service.GetGenericTypeDefinition().Name : service.Name;
        return new OpcUaEntityServiceConfigurationException(
            $"The Entity {entity.Name} does not have any {missingService} configured!");
    }
}