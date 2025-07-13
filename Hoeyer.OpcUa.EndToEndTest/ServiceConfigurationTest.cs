using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Hoeyer.OpcUa.TestEntities;
using Hoeyer.OpcUa.TestEntities.Methods;
using Hoeyer.OpcUa.TestEntities.Methods.Generated;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest;

[ClassDataSource<OpcFullSetupWithBackgroundServerFixture>]
public sealed class ServiceConfigurationTest(OpcFullSetupWithBackgroundServerFixture serverFixture)
{
    [Test]
    public async Task EntityBrowser_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(serverFixture.Services, typeof(IEntityBrowser<>));

    [Test]
    public async Task EntityTranslator_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(serverFixture.Services, typeof(IEntityTranslator<>));


    [Test]
    public async Task EntityInitializer_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(serverFixture.Services,
            typeof(IManagedEntityNodeSingletonFactory<>));


    [Test]
    public async Task EntityMonitor_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(serverFixture.Services,
            typeof(IEntitySubscriptionManager<>));


    [Test]
    [ServiceCollectionDataSource]
    [TestSubject(typeof(IEntitySubscriptionManager<>))]
    public async Task EntityMonitor_IsOnlyRegisteredAsSingleton(IServiceCollection descriptors)
    {
        IEnumerable<ServiceDescriptor> services =
            await AssertNumberEntitiesMatchesNumberServices(descriptors, typeof(IEntitySubscriptionManager<>));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [ServiceCollectionDataSource]
    [TestSubject(typeof(IEntityNodeManagerFactory<>))]
    public async Task EntityNodeManagerFactory_Generic_AreSingleton(IServiceCollection descriptors)
    {
        IEnumerable<ServiceDescriptor> services =
            await AssertNumberEntitiesMatchesNumberServices(descriptors, typeof(IEntityNodeManagerFactory<>));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [TestSubject(typeof(IEntityNodeManagerFactory))]
    [DisplayName("Node manager factory (non-generic) - number of singletons")]
    [ServiceCollectionDataSource]
    public async Task EntityNodeManagerFactory_NonGeneric_AreSingleton(IServiceCollection descriptors)
    {
        var services = await AssertNumberEntitiesMatchesNumberServices(descriptors, typeof(IEntityNodeManagerFactory));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [ClassDataSource<ApplicationFixture>]
    public async Task When_ActionSimulatorIsImplemented_ItIsRegistered(ApplicationFixture fixture)
    {
        await fixture.GetService<EntityServerStartedMarker>();
        var simulator = fixture.GetService<ISimulation<Gantry, PickUpContainerArgs>>();
        await Assert.That(simulator).IsNotNull();
    }

    [Test]
    [ClassDataSource<ApplicationFixture>]
    public async Task When_FunctionSimulatorIsImplemented_ItIsRegistered(ApplicationFixture fixture)
    {
        await fixture.GetService<EntityServerStartedMarker>();
        var simulator = fixture.GetService<ISimulation<Gantry, GetCurrentContainerIdArgs, Guid>>();
        await Assert.That(simulator).IsNotNull();
    }


    [Test]
    [ServiceCollectionDataSource]
    [TestSubject(typeof(IGantryMethods))]
    public async Task GantryMethodsInterface_ShouldBeRegistered(IGantryMethods methodInterface)
        => await Assert.That(methodInterface).IsNotNull().Because(" the interface definition should be wired.");

    [Test]
    [ServiceCollectionDataSource]
    [TestSubject(typeof(ServiceCollectionExtension))]
    [TestSubject(typeof(SimulationAdaptionServiceExtensions))]
    public async Task CanProvide_SimulationServerAdapters_SimulationNoReturn(IServiceCollection collection)
    {
        var provider = collection.BuildServiceProvider().CreateAsyncScope().ServiceProvider;
        var adapter = provider.GetRequiredService<ActionSimulationAdapter<Gantry, ChangePositionArgs>>();
        var configurators = provider.GetRequiredService<IEnumerable<INodeConfigurator<Gantry>>>();

        var wantedName = typeof(EntityStateChangedNotifier<>).Name;
        await Assert.That(configurators.Where(e => wantedName.Contains(e.GetType().Name))).IsNotEmpty();
    }

    [Test]
    [ServiceCollectionDataSource]
    [TestSubject(typeof(ServiceCollectionExtension))]
    [TestSubject(typeof(SimulationAdaptionServiceExtensions))]
    public async Task EntityStateChangedNotifier_IsRegisteredAs_ScopedIActionSimulationProcessor(
        IServiceCollection collection)
    {
        var foundDescriptor = collection.FirstOrDefault(e =>
            e.Lifetime == ServiceLifetime.Scoped && e.ImplementationType.GetGenericTypeDefinition() !=
            typeof(EntityStateChangedNotifier<>));

        await Assert.That(foundDescriptor).IsNotNull().Because("Any " + nameof(EntityStateChangedNotifier<int>) +
                                                               " should be registered as " +
                                                               typeof(IStateChangeSimulationProcessor<>).Name);
    }

    private static async Task<IEnumerable<ServiceDescriptor>> AssertNumberEntitiesMatchesNumberServices(
        IEnumerable<ServiceDescriptor> collection,
        Type wantedType)
    {
        Predicate<Type> typeFilter = wantedType.IsGenericTypeDefinition
            ? serviceType =>
                serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == wantedType
            : serviceType => serviceType.IsAssignableFrom(wantedType);

        await Assert.That(OpcUaEntityTypes.Entities.Count).IsNotZero()
            .Because(" there must be entities for services to be generated");
        var services = collection.Where(e => typeFilter.Invoke(e.ServiceType)).ToList();
        await Assert.That(services).IsNotEmpty().Because(" there should be at least one service registered");
        await Assert.That(services.Count).IsEqualTo(OpcUaEntityTypes.Entities.Count);
        return services;
    }
}