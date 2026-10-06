using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Fixtures.Common.TUnit.Configuration.DisplayName;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Fixtures.Common.Test;

[DisplayName("Dependency resolution test")]
public abstract class DependencyResolutionTest(IServiceCollection services, Type assemblyMarker)
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
            Categories: [serviceDescriptor.Lifetime.ToString(), .. additionalCategories]
        );
    }

    [Test]
    [ArgumentDisplayFormatter<ServiceDescriptorFormatter>]
    [DisplayName("The generic service $descriptor should be resolvable within a scope")]
    [InstanceMethodDataSource(nameof(ProvidableWithScope))]
    public async Task ShouldBeProvidableWithScope(ServiceDescriptor descriptor)
    {
        await using var asyncScope = services.BuildServiceProvider().CreateAsyncScope();
        var provider = asyncScope.ServiceProvider;
        await Assert.That(() => provider.GetRequiredService(descriptor.ServiceType)).ThrowsNothing();
    }

    [Test]
    [ArgumentDisplayFormatter<ServiceDescriptorFormatter>]
    [DisplayName("The generic service $descriptor should be resolvable with no scope")]
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
    [ArgumentDisplayFormatter<ServiceDescriptorFormatter>]
    [DisplayName("The service $descriptor should be resolvable within a scope")]
    [InstanceMethodDataSource(nameof(GenericsProvidableWithScope))]
    public async Task GenericShouldBeProvidableWithScope(ServiceDescriptor descriptor)
    {
        await using var asyncScope = services.BuildServiceProvider().CreateAsyncScope();
        var provider = asyncScope.ServiceProvider;
        var serviceType = descriptor.ServiceType.MakeGenericType(assemblyMarker);

        await Assert.That(() => provider.GetRequiredService(serviceType)).ThrowsNothing();
    }

    [Test]
    [ArgumentDisplayFormatter<ServiceDescriptorFormatter>]
    [DisplayName("The service $descriptor should be resolvable without a scope")]
    [InstanceMethodDataSource(nameof(GenericsProvidableWithNoScope))]
    public async Task GenericShouldBeProvidableWithNoScope(ServiceDescriptor descriptor)
    {
        var provider = services.BuildServiceProvider();
        var serviceType = descriptor.ServiceType.MakeGenericType(assemblyMarker);
        await Assert.That(() => provider.GetRequiredService(serviceType)).ThrowsNothing();
    }
}