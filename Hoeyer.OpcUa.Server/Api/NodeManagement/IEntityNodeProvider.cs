namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IEntityNodeProvider<T>
{
    /// <summary>
    /// A provider which provides an <see cref="IManagedEntityNode"/> to the consumer 
    /// </summary>
    /// <returns>The managed node representing the entity</returns>
    /// <exception cref="EntityNodeProviderException">if the node could not be provided</exception>
    public IManagedEntityNode<T> GetEntityNode();
}