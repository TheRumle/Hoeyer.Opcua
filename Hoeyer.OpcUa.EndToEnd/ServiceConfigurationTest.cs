using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest;

[ClassDataSource<AllOpcUaServicesFixture>]
public sealed class ServiceConfigurationTest(AllOpcUaServicesFixture fixture)
{
    [Test]
    public async Task EntityBrowser_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityBrowser<>));

    [Test]
    public async Task EntityTranslator_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityTranslator<>));


    [Test]
    public async Task EntityInitializer_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services,
            typeof(IManagedEntityNodeSingletonFactory<>));



    [Test]
    public async Task EntityChanged_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityChangedBroadcaster<>));

    [Test]
    public async Task EntityMonitor_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntitySubscriptionManager<>));


    [Test]
    [AllEntityServiceDescriptorsOfType(typeof(IEntitySubscriptionManager<>))]
    public async Task EntityMonitor_IsOnlyRegisteredAsSingleton(IReadOnlyCollection<ServiceDescriptor> descriptors)
    {
        var numberOfSingletons = descriptors.Count(e => e.Lifetime == ServiceLifetime.Singleton);
        await Assert.That(numberOfSingletons).IsEqualTo(OpcUaEntityTypes.Entities.Count);
        await Assert.That(descriptors.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [AllEntityServiceDescriptorsOfType(typeof(IEntityNodeManagerFactory<>))]
    public async Task EntityNodeManagerFactory_Generic_AreSingleton(IReadOnlyCollection<ServiceDescriptor> descriptors)
    {
        var numberOfSingletons = descriptors.Count(e => e.Lifetime == ServiceLifetime.Singleton);
        await Assert.That(numberOfSingletons).IsEqualTo(OpcUaEntityTypes.Entities.Count);
        await Assert.That(descriptors.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [AllEntityServiceDescriptorsOfType(typeof(IEntityNodeManagerFactory))]
    public async Task EntityNodeManagerFactory_NonGeneric_AreSingleton(
        IReadOnlyCollection<ServiceDescriptor> descriptors)
    {
        var numberOfSingletons = descriptors.Count(e => e.Lifetime == ServiceLifetime.Singleton);
        await Assert.That(numberOfSingletons).IsEqualTo(OpcUaEntityTypes.Entities.Count);
        await Assert.That(descriptors.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [AllEntityServiceDescriptorsOfType(typeof(IEntityChangedBroadcaster<>))]
    public async Task EntityChangedMessenger_IsOnlyRegisteredAsSingleton(
        IReadOnlyCollection<ServiceDescriptor> descriptors)
    {
        var numberOfSingletons = descriptors.Count(e => e.Lifetime == ServiceLifetime.Singleton);
        await Assert.That(numberOfSingletons).IsEqualTo(OpcUaEntityTypes.Entities.Count);
        await Assert.That(descriptors.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }


    private static async Task AssertNumberEntitiesMatchesNumberServices(IEnumerable<ServiceDescriptor> collection,
        Type wantedType)
    {
        Predicate<Type> typeFilter = wantedType.IsGenericTypeDefinition
            ? serviceType =>
                serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == wantedType
            : serviceType => serviceType.IsAssignableFrom(wantedType);

        var services = collection.Where(e => typeFilter.Invoke(e.ServiceType)).ToList();
        await Assert.That(services).IsNotEmpty();
        await Assert.That(services.Count).IsEqualTo(OpcUaEntityTypes.Entities.Count);
    }
}