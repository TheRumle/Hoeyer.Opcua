using System;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity;

/// <summary>
/// Represents the root configurations used to set up OpcUa services for a configurable entity. 
/// </summary>
/// <param name="Id">A unique identifier for this specific entity within the OpcUa server.</param>
/// <param name="NameSpaceIndex">The index of the global namespace for the configuration/application. See <see href="https://reference.opcfoundation.org/v104/Core/docs/Part3/8.2.2/"/> for details on namespaces and their usage.</param>
public record RootIdentity(string Id, int NameSpaceIndex)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RootIdentity"/> record with a GUID identifier.
    /// </summary>
    /// <param name="id">A unique GUID identifying this specific entity within the OpcUa server.</param>
    /// <param name="nameSpace">The index of the global namespace for the configuration.</param>
    public RootIdentity(Guid id, int nameSpace) : this(id.ToString(), nameSpace)
    {}

    public string Id { get; } = Id;
    public int NameSpaceIndex { get; } = NameSpaceIndex;
}