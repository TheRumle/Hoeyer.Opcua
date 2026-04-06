using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IManagedEntityNodeProvider<T>
{
    IManagedEntityNode<T> Node { get; }
    Task<IManagedEntityNode<T>> GetOrCreateManagedEntityNode(ushort namespaceIndex, string @namespace);
}