using System;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IManagedAgentSingletonFactory<T>
{
    IManagedAgent? Node { get; }
    Task<IManagedAgent<T>> CreateManagedAgent(Func<string, ushort> namespaceToIndex);
}