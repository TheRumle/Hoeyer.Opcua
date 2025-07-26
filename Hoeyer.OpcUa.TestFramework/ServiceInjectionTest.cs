using Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Opc.Ua.Test.TUnit;

//TODO fix when TUnit works
public abstract class ServiceInjectionTest(
    IServiceCollection collection,
    IEnumerable<IPartialServiceMatcher> allMatchers)
{
    private List<IPartialServiceMatcher> MatchersWithLifetime(ServiceLifetime lifetime)
        => allMatchers.Where(d => d.Lifetime == lifetime).ToList();

    private IEnumerable<ServiceDescriptor> GetMatchingDescriptors(IEnumerable<IPartialServiceMatcher> matchers) =>
        from descriptor in collection
        from matcher in matchers
        where matcher.Equals(descriptor)
        select descriptor;

    public IEnumerable<ServiceDescriptor> SingletonServices() =>
        GetMatchingDescriptors(MatchersWithLifetime(ServiceLifetime.Singleton));

    public IEnumerable<ServiceDescriptor> TransientServices() =>
        GetMatchingDescriptors(MatchersWithLifetime(ServiceLifetime.Transient));

    public IEnumerable<ServiceDescriptor> ScopedServices() =>
        GetMatchingDescriptors(MatchersWithLifetime(ServiceLifetime.Scoped));

    public IEnumerable<IPartialServiceMatcher> AllDescriptors() => allMatchers;


    [Test]
    [DisplayName("The service '$descriptor.ServiceType' can be provided as singleton.")]
    [InstanceMethodDataSource<ServiceInjectionTest>(nameof(SingletonServices))]
    public async Task ServiceCanBeCreatedAsSingleton(ServiceDescriptor descriptor) =>
        await AssertServiceCanBeCreated(descriptor);

    [Test]
    [DisplayName("The service '$descriptor.ServiceType' can be provided as singleton.")]
    [InstanceMethodDataSource<ServiceInjectionTest>(nameof(TransientServices))]
    public async Task ServiceCanBeCreatedAsTransient(ServiceDescriptor descriptor) =>
        await AssertServiceCanBeCreated(descriptor);

    [Test]
    [DisplayName("The service '$descriptor.ServiceType' can be provided as scoped.")]
    [InstanceMethodDataSource<ServiceInjectionTest>(nameof(ScopedServices))]
    public async Task ServiceCanBeCreatedAsScoped(ServiceDescriptor descriptor) =>
        await AssertServiceCanBeCreatedScoped(descriptor);


    [Test]
    [DisplayName("A service matching $matcher has been registered")]
    [InstanceMethodDataSource<ServiceInjectionTest>(nameof(AllDescriptors))]
    public async Task ServiceIsRegistered(IPartialServiceMatcher matcher)
    {
        await Assert.That(collection.Any(matcher.Equals)).IsTrue();
    }


    private async Task AssertServiceCanBeCreated(ServiceDescriptor descriptor)
    {
        var provider = collection.BuildServiceProvider();
        var foundService = provider.GetService(descriptor.ServiceType);
        await Assert.That(foundService).IsNotNull().Because("We want to construct the service as a singleton");
    }

    private async Task AssertServiceCanBeCreatedScoped(ServiceDescriptor descriptor)
    {
        var provider = collection.BuildServiceProvider().CreateAsyncScope();
        var foundService = provider.ServiceProvider.GetService(descriptor.ServiceType);
        await Assert.That(foundService).IsNotNull().Because("We want to construct the service as a singleton");
    }
}