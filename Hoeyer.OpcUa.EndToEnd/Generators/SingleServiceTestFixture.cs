using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class SingleServiceTestFixture<TWanted>(Func<ApplicationFixture, Task<TWanted>> providerFunc) : IAsyncDisposable, IAsyncInitializer
{
    ApplicationFixture _applicationFixture = new();
    
    public async Task<T> GetService<T>() where T : notnull =>  await _applicationFixture.GetService<T>()!;

    /// <inheritdoc />
    public async ValueTask DisposeAsync() => await _applicationFixture.DisposeAsync();

    /// <inheritdoc />
    public async Task InitializeAsync() => await _applicationFixture.InitializeAsync();

    public async Task<TWanted> GetClassUnderTest() => await providerFunc.Invoke(_applicationFixture);
}