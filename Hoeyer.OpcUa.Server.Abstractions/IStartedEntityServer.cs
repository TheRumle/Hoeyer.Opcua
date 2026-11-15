namespace Hoeyer.OpcUa.Server.Abstractions;

public interface IStartedEntityServer : IAsyncDisposable
{
    IOpcEntityServer Server { get; }
}