namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public sealed class InvalidEntityConfigurationException(string entity, string message)
    : Exception(entity + " was misconfigured: " + message);