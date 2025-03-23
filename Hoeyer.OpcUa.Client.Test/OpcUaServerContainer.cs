using DotNet.Testcontainers.Containers;

namespace Hoeyer.OpcUa.ClientTest;

public sealed record OpcUaServerContainer(IContainer ServerContainer, int PortToServer) : IAsyncDisposable
{
    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await ServerContainer.DisposeAsync();
    }
}