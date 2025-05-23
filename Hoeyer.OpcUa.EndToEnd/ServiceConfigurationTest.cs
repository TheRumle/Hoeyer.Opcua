﻿using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using JetBrains.Annotations;
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
    [ClassDataSource<AllOpcUaServicesFixture>]
    [TestSubject(typeof(IEntitySubscriptionManager<>))]
    public async Task EntityMonitor_IsOnlyRegisteredAsSingleton(List<ServiceDescriptor> descriptors)
    {
        var services = await AssertNumberEntitiesMatchesNumberServices(descriptors, typeof(IEntitySubscriptionManager<>));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [ClassDataSource<AllOpcUaServicesFixture>]
    [TestSubject(typeof(IEntityNodeManagerFactory<>))]
    public async Task EntityNodeManagerFactory_Generic_AreSingleton(List<ServiceDescriptor> descriptors)
    {
        var services = await AssertNumberEntitiesMatchesNumberServices(descriptors, typeof(IEntityNodeManagerFactory<>));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [TestSubject(typeof(IEntityNodeManagerFactory))]
    [DisplayName("Node manager factory (non-generic) - number of singletons")]
    [ClassDataSource<AllOpcUaServicesFixture>]
    public async Task EntityNodeManagerFactory_NonGeneric_AreSingleton(List<ServiceDescriptor> descriptors)
    {
        var services = await AssertNumberEntitiesMatchesNumberServices(descriptors, typeof(IEntityNodeManagerFactory));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [ClassDataSource<AllOpcUaServicesFixture>]
    [TestSubject(typeof(IEntityChangedBroadcaster<>))]
    public async Task EntityChangedMessenger_IsOnlyRegisteredAsSingleton(
        List<ServiceDescriptor> descriptors)
    {
        var services = await AssertNumberEntitiesMatchesNumberServices(descriptors, typeof(IEntityChangedBroadcaster<>));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }


    private static async Task<IEnumerable<ServiceDescriptor>> AssertNumberEntitiesMatchesNumberServices(
        IEnumerable<ServiceDescriptor> collection,
        Type wantedType)
    {
        Predicate<Type> typeFilter = wantedType.IsGenericTypeDefinition
            ? serviceType =>
                serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == wantedType
            : serviceType => serviceType.IsAssignableFrom(wantedType);

        var services = collection.Where(e => typeFilter.Invoke(e.ServiceType)).ToList();
        await Assert.That(services).IsNotEmpty().Because(" there should be at least one service registered");
        await Assert.That(services.Count).IsEqualTo(OpcUaEntityTypes.Entities.Count);
        return services;
    }
}