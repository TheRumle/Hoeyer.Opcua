namespace Hoeyer.OpcUa.Server.Abstractions.NodeManagement;

public interface IManagedEntityNodeProvider<T>
{
    IManagedEntityNode<T> Node { get; }
    Task<IManagedEntityNode<T>> GetOrCreateManagedEntityNode(ushort namespaceIndex, string @namespace);
}