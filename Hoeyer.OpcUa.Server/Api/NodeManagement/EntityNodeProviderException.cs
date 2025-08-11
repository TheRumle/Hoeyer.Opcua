using System;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

/// <summary>
/// An exception indicating that an <see cref="IEntityNodeProvider{T}"/> has been registered for the entity but is unable to get the node.
/// This exception is thrown if, for instance, an <see cref="IEntityNodeManager"/> has not been configured for the node. 
/// </summary>
public sealed class EntityNodeProviderException(string s) : Exception(s)
{
    public EntityNodeProviderException(Type entity) : this(
        $"An IEntityNodeProvider has been registered for the entity '{entity.FullName}', but the provider could not get the entity.")
    {
    }

    public EntityNodeProviderException(Type entity, string because) : this(
        $"An IEntityNodeProvider has been registered for the entity '{entity.FullName}', but the provider could not get the entity. " +
        because)
    {
    }
}