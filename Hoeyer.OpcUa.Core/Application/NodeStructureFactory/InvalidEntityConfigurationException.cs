using System;

namespace Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

public sealed class InvalidEntityConfigurationException(string entity, string message)
    : Exception(entity + " was misconfigured: " + message);