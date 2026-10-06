using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Fixtures.Common.Utils;


public sealed class TestServiceCollection
{
    public readonly IServiceProvider ServiceProvider;

    public TestServiceCollection(Action<IServiceCollection> configure)
    {
        IServiceCollection collection = new ServiceCollection();
        configure(collection);
        foreach (var descriptor in collection.Where(NotNonOwnedType).ToArray())
        {
            var nonOwnedType = typeof(NonOwned<>).MakeGenericType(descriptor.ServiceType);
            collection.Add(new ServiceDescriptor(
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
        collection.AddSingleton(collection);
        ServiceProvider = collection.BuildServiceProvider();
    }

    private static bool NotNonOwnedType(ServiceDescriptor descriptor) =>
        !(descriptor.ServiceType.IsGenericType &&
        descriptor.ServiceType.GetGenericTypeDefinition() == typeof(NonOwned<>));
}