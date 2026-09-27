using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;

public sealed class IntegrationTestServiceProvider : IAsyncDisposable
{
    public readonly IServiceProvider SingletonProvider;

    public IntegrationTestServiceProvider(IServiceProvider singletonProvider)
    {
        SingletonProvider = singletonProvider;
    }

    public async ValueTask DisposeAsync()
    {
        if (SingletonProvider is IAsyncDisposable serviceScopeAsyncDisposable)
        {
            await serviceScopeAsyncDisposable.DisposeAsync();
        }
        else if (SingletonProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public object Create(IServiceScope scope, Type type)
    {
        var nonOwned = typeof(NonOwned<>).MakeGenericType(type);
        if (type.IsAssignableTo(nonOwned))
        {
            var nonOwnedInner = scope.ServiceProvider.GetRequiredService(type);
            return Activator.CreateInstance(nonOwned, nonOwnedInner)
                   ?? throw new InvalidOperationException(
                       $"Could not create {type}.");
        }

        object? singletonService = SingletonProvider.GetService(type);
        if (singletonService is IDisposable or IAsyncDisposable)
        {
            throw new InvalidOperationException(
                $"Test member '{type.Name}' resolves to {nameof(IDisposable)}/{nameof(IAsyncDisposable)} singleton for this test session.as a session." +
                "The test runner disposes injected disposables when and this would dispose the session-singleton before all tests are finished. " +
                $"Inject a {nameof(NonOwned<>)} of type {type.Name} to borrow it, or resolve a test-owned instance inside the test body.");
        }

        if (singletonService is not null) return singletonService;
        return scope.ServiceProvider.GetRequiredService(type);
    }
}