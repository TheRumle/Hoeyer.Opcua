using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Fixtures.Common.Utils;


public sealed class TestServiceCollection : IAsyncDisposable
{
    public readonly IServiceProvider ServiceProvider;

    public TestServiceCollection(IServiceCollection collection)
    {
        IServiceCollection newCollection = new ServiceCollection();
        foreach (var descriptor in collection.Where(NotNonOwnedType).ToArray())
        {
            newCollection.Add(descriptor);
            var nonOwnedType = typeof(NonOwned<>).MakeGenericType(descriptor.ServiceType);
            newCollection.Add(new ServiceDescriptor(
                nonOwnedType,
                sp =>
                {
                    var service = sp.GetRequiredService(descriptor.ServiceType);
                    return Activator.CreateInstance(nonOwnedType, service)
                           ?? throw new InvalidOperationException(
                               $"Could not create {nonOwnedType}.");
                },
                descriptor.Lifetime));
        }
        
        ServiceProvider = newCollection.BuildServiceProvider();
    }

    private static bool NotNonOwnedType(ServiceDescriptor descriptor) =>
        !(descriptor.ServiceType.IsGenericType &&
        descriptor.ServiceType.GetGenericTypeDefinition() == typeof(NonOwned<>));

    public async ValueTask DisposeAsync()
    {
        if (ServiceProvider is IAsyncDisposable serviceScopeAsyncDisposable)
        {
            await serviceScopeAsyncDisposable.DisposeAsync();
        }
        else if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}