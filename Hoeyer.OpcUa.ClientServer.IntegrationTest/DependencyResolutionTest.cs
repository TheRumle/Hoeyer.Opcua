using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest;

[ServiceInjection]
public sealed class DependencyResolutionTest(IServiceCollection services)
{
    private IEnumerable<ServiceDescriptor> Descriptors =>
        services.Where(e => e.ImplementationType is { ContainsGenericParameters: false });

    private IEnumerable<ServiceDescriptor> GenericDescriptors =>
        services.Where(e =>
            e.ServiceType.IsGenericTypeDefinition &&
            e.ServiceType.GetGenericArguments().Length == 1 &&
            e.ImplementationType is { ContainsGenericParameters: true });

    public IEnumerable<TestDataRow<ServiceDescriptor>> ProvidableWithNoScope()
    {
        ServiceLifetime[] lifetimes = [ServiceLifetime.Transient, ServiceLifetime.Singleton];
        return Descriptors
            .Where(descriptor => lifetimes.Contains(descriptor.Lifetime))
            .Select(CreateDataRow);
    }

    public IEnumerable<TestDataRow<ServiceDescriptor>> ProvidableWithScope()
    {
        ServiceLifetime[] lifetimes = [ServiceLifetime.Transient, ServiceLifetime.Singleton, ServiceLifetime.Scoped];
        return Descriptors
            .Where(descriptor => lifetimes.Contains(descriptor.Lifetime))
            .Select(CreateDataRow);
    }

    private static string DescriptionOf(ServiceDescriptor s)
    {
        var implType = s.ImplementationType?.GetFriendlyTypeName() ?? "null";
        return $"[ServiceType: {s.ServiceType.GetFriendlyTypeName()}, ImplType: {implType}, Lifetime: {s.Lifetime}]";
    }

    private static TestDataRow<ServiceDescriptor> CreateDataRow(
        ServiceDescriptor serviceDescriptor)
    {
        string[] additionalCategories = serviceDescriptor.ServiceType.IsGenericTypeDefinition
            ? ["Open generic"]
            : [];

        return new(
            serviceDescriptor,
            DisplayName: DescriptionOf(serviceDescriptor),
            Categories: [serviceDescriptor.Lifetime.ToString(), ..additionalCategories]
        );
    }

    [Test]
    [InstanceMethodDataSource(nameof(ProvidableWithScope))]
    public async Task ShouldBeProvidableWithScope(ServiceDescriptor descriptor)
    {
        await using var asyncScope = services.BuildServiceProvider().CreateAsyncScope();
        var provider = asyncScope.ServiceProvider;
        await Assert.That(() => provider.GetRequiredService(descriptor.ServiceType)).ThrowsNothing();
    }

    [Test]
    [InstanceMethodDataSource(nameof(ProvidableWithNoScope))]
    public async Task ShouldBeProvidableWithNoScope(ServiceDescriptor descriptor)
    {
        var provider = services.BuildServiceProvider();
        await Assert.That(() => provider.GetRequiredService(descriptor.ServiceType)).ThrowsNothing();
    }

    public IEnumerable<TestDataRow<ServiceDescriptor>> GenericsProvidableWithNoScope()
    {
        ServiceLifetime[] lifetimes = [ServiceLifetime.Transient, ServiceLifetime.Singleton];
        return GenericDescriptors
            .Where(descriptor => lifetimes.Contains(descriptor.Lifetime))
            .Select(CreateDataRow);
    }

    public IEnumerable<TestDataRow<ServiceDescriptor>> GenericsProvidableWithScope()
    {
        ServiceLifetime[] lifetimes = [ServiceLifetime.Transient, ServiceLifetime.Singleton, ServiceLifetime.Scoped];
        return GenericDescriptors
            .Where(descriptor => lifetimes.Contains(descriptor.Lifetime))
            .Select(CreateDataRow);
    }


    [Test]
    [InstanceMethodDataSource(nameof(GenericsProvidableWithScope))]
    public async Task GenericShouldBeProvidableWithScope(ServiceDescriptor descriptor)
    {
        await using var asyncScope = services.BuildServiceProvider().CreateAsyncScope();
        var provider = asyncScope.ServiceProvider;
        var serviceType = descriptor.ServiceType.MakeGenericType(typeof(TestEntity));

        await Assert.That(() => provider.GetRequiredService(serviceType)).ThrowsNothing();
    }

    [Test]
    [InstanceMethodDataSource(nameof(GenericsProvidableWithNoScope))]
    public async Task GenericShouldBeProvidableWithNoScope(ServiceDescriptor descriptor)
    {
        var provider = services.BuildServiceProvider();
        var serviceType = descriptor.ServiceType.MakeGenericType(typeof(TestEntity));
        await Assert.That(() => provider.GetRequiredService(serviceType)).ThrowsNothing();
    }
}