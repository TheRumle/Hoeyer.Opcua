namespace Hoeyer.OpcUa.Server;

public sealed class DuplicateSetupException(Type entityType)
    : Exception(
        $"The address space for entity '{entityType.Name}' was set up more than once. " +
        "A node manager's address space must only be created once per server instance.");