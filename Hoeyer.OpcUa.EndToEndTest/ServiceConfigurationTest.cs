using Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Hoeyer.OpcUa.TestEntities.Methods;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest;

[ClassDataSource<OpcFullSetupWithBackgroundServerFixtureAttribute>]
public sealed class ServiceConfigurationTest(OpcFullSetupWithBackgroundServerFixtureAttribute fixture)
{
    private readonly IEnumerable<ServiceDescriptor> _descriptors = fixture.Services;

    [Test]
    [TestSubject(typeof(IEntityBrowser<>))]
    [DisplayName("Entity browser is registered per entity")]
    public async Task EntityBrowser_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityBrowser<>));

    [Test]
    [TestSubject(typeof(IEntityTranslator<>))]
    [DisplayName("Entity translator is registered per entity")]
    public async Task EntityTranslator_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityTranslator<>));


    [Test]
    [TestSubject(typeof(IManagedEntityNodeSingletonFactory<>))]
    [DisplayName("Managed entity node singleton factory is registered per entity")]
    public async Task IManagedEntityNodeSingletonFactory_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services,
            typeof(IManagedEntityNodeSingletonFactory<>));


    [Test]
    [TestSubject(typeof(IEntitySubscriptionManager<>))]
    [DisplayName("Entity monitor (subscription manager) is registered per entity")]
    public async Task EntityMonitor_IsRegistered()
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services,
            typeof(IEntitySubscriptionManager<>));


    [Test]
    [TestSubject(typeof(IEntitySubscriptionManager<>))]
    [DisplayName("Entity monitor is only registered as singleton")]
    public async Task EntityMonitor_IsOnlyRegisteredAsSingleton()
    {
        IEnumerable<ServiceDescriptor> services =
            await AssertNumberEntitiesMatchesNumberServices(_descriptors, typeof(IEntitySubscriptionManager<>));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [TestSubject(typeof(IEntityNodeManagerFactory<>))]
    [DisplayName("Entity node manager factory (generic) is registered as singleton per entity")]
    public async Task EntityNodeManagerFactory_Generic_AreSingleton()
    {
        IEnumerable<ServiceDescriptor> services =
            await AssertNumberEntitiesMatchesNumberServices(_descriptors, typeof(IEntityNodeManagerFactory<>));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [TestSubject(typeof(IEntityNodeManagerFactory))]
    [DisplayName("Entity node manager factory (non-generic) is registered as singleton")]
    public async Task EntityNodeManagerFactory_NonGeneric_AreSingleton()
    {
        var services = await AssertNumberEntitiesMatchesNumberServices(_descriptors, typeof(IEntityNodeManagerFactory));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [ServiceCollectionDataSource]
    [TestSubject(typeof(IGantryMethods))]
    [DisplayName("Gantry methods interface is registered")]
    public async Task GantryMethodsInterface_ShouldBeRegistered(IGantryMethods methodInterface)
        => await Assert.That(methodInterface).IsNotNull().Because(" the interface definition should be wired.");

    [Test]
    [TestSubject(typeof(ServiceCollectionExtension))]
    [TestSubject(typeof(ServerSimulationAdapter))]
    [DisplayName("Simulation server adapters are registered and resolvable")]
    public async Task CanProvide_SimulationServerAdapters_SimulationNoReturn()
    {
        var collection = fixture.ServiceCollection;
        IPartialServiceMatcher functionMatcher = new DoubleGenericPredicateMatcher(
            typeof(INodeConfigurator<>),
            typeof(FunctionSimulationAdapter<,,>));

        IPartialServiceMatcher actionMatcher = new DoubleGenericPredicateMatcher(
            typeof(INodeConfigurator<>),
            typeof(ActionSimulationAdapter<,>));

        var functionAdapters = collection.Where(functionMatcher).ToList();
        var actionAdapters = collection.Where(actionMatcher).ToList();

        using (Assert.Multiple())
        {
            await Assert.That(functionAdapters).IsNotEmpty()
                .Because("At least one " + "SimulationAdapter<T,T,T>" + " should be registered");

            await Assert.That(actionAdapters).IsNotEmpty()
                .Because(
                    "At least one SimulationAdapter<T,T> adapter between simulation and service collection should exist");

            var provider = collection.BuildServiceProvider().CreateAsyncScope().ServiceProvider;
            var adaptersByService = functionAdapters.Union(actionAdapters).GroupBy(s => s.ServiceType);
            foreach (var serviceGroup in adaptersByService.Where(e => e.Count() > 1))
            {
                var castedAdapters =
                    provider.GetService(typeof(IEnumerable<>).MakeGenericType(serviceGroup.Key)) as IEnumerable<object>;
                var wantedAdapters = castedAdapters == null ? [] : castedAdapters.ToList();
                await Assert.That(wantedAdapters).IsNotEmpty()
                    .Because("adapters with same service registration should be registered as IEnumerable.");
            }
        }
    }

    [Test]
    [TestSubject(typeof(ServiceCollectionExtension))]
    [TestSubject(typeof(ServerSimulationAdapter))]
    [DisplayName("Entity state changed notifier is registered as scoped action simulation processor")]
    public async Task EntityStateChangedNotifier_IsRegisteredAs_ScopedIActionSimulationProcessor()
    {
        var foundDescriptor = _descriptors.FirstOrDefault(e =>
            e is { Lifetime: ServiceLifetime.Scoped, ImplementationType: not null }
            && e.ImplementationType.GetGenericTypeDefinition() != typeof(EntityStateChangedNotifier<>));

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