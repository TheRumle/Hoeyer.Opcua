using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Abstractions;

public interface IStartedEntityServer : IAsyncDisposable
{
    ISystemContext SystemContext { get; }
}