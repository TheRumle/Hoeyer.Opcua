using System;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IManagedEntityNodeSingletonFactory<T> : IEntityNodeProvider<T>
{
    IManagedEntityNode? Node { get; }
    Task<IManagedEntityNode<T>> CreateManagedEntityNode(Func<string, ushort> namespaceToIndex);
}