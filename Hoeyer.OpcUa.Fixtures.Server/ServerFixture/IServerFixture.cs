using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Fixtures.Server.ServerFixture;

public interface IServerFixture : IAsyncInitializer, IAsyncDisposable
{
    public IServiceProvider AvailableServices { get; }
}