using System;

namespace Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

public sealed class InvalidAgentConfigurationException(string agent, string message)
    : Exception(agent + " was misconfigured: " + message);