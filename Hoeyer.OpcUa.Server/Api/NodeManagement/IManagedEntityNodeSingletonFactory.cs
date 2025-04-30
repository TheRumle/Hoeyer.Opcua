using System;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IManagedEntityNodeSingletonFactory<T>
{
    IManagedEntityNode? Node { get; }
    Task<IManagedEntityNode> CreateManagedEntityNode(Func<string, ushort> namespaceToIndex);
}

