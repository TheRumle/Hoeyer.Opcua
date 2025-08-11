using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public class OpcUaAgentServiceConfigurationException(string message) : Exception(message)
{
    public OpcUaAgentServiceConfigurationException(
        IEnumerable<OpcUaAgentServiceConfigurationException> configurationExceptions) :
        this(
            string.Join("\n", configurationExceptions.Select(e => e.Message)))
    {
    }

    public OpcUaAgentServiceConfigurationException(
        IEnumerable<Exception> configurationExceptions) :
        this(string.Join("\n", configurationExceptions.Select(e => e.Message)))
    {
    }


    public OpcUaAgentServiceConfigurationException(
        params OpcUaAgentServiceConfigurationException[] configurationExceptions) :
        this(string.Join("\n", configurationExceptions.Select(e => e.Message)))
    {
    }

    public static OpcUaAgentServiceConfigurationException ServiceNotConfigured(Type agent, Type service)
    {
        var missingService = service.IsGenericType ? service.GetGenericTypeDefinition().Name : service.Name;
        return new OpcUaAgentServiceConfigurationException(
            $"The Agent {agent.Name} does not have any {missingService} configured!");
    }
}