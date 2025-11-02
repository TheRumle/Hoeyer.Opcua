using System;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class InvalidEntityConfigurationException(string entity, string message)
    : Exception(entity + " was misconfigured: " + message);