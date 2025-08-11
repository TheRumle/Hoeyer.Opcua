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
            typeof(IManagedAgentSingletonFactory<>));


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
    [TestSubject(typeof(IAgentManagerFactory<>))]
    public async Task AgentManagerFactory_Generic_AreSingleton(IServiceCollection descriptors)
    {
        IEnumerable<ServiceDescriptor> services =
            await AssertNumberEntitiesMatchesNumberServices(descriptors, typeof(IAgentManagerFactory<>));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [TestSubject(typeof(IAgentManagerFactory))]
    [DisplayName("Node manager factory (non-generic) - number of singletons")]
    [ServiceCollectionDataSource]
    public async Task AgentManagerFactory_NonGeneric_AreSingleton(IServiceCollection descriptors)
    {
        var services = await AssertNumberEntitiesMatchesNumberServices(descriptors, typeof(IAgentManagerFactory));
        await Assert.That(services.Where(e => e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }

    [Test]
    [ServiceCollectionDataSource]
    [TestSubject(typeof(IGantryMethods))]
    public async Task GantryMethodsInterface_ShouldBeRegistered(IGantryMethods methodInterface)
        => await Assert.That(methodInterface).IsNotNull().Because(" the interface definition should be wired.");

    [Test]
    [ServiceCollectionDataSource]
    [TestSubject(typeof(ServiceCollectionExtension))]
    [TestSubject(typeof(ServerSimulationAdapter))]
    public async Task CanProvide_SimulationServerAdapters_SimulationNoReturn(IServiceCollection collection)
    {
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
                .Because("At least one function adapter between simulation and service collection should exist");

            await Assert.That(actionAdapters).IsNotEmpty()
                .Because("At least one action adapter between simulation and service collection should exist");

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
    [ServiceCollectionDataSource]
    [TestSubject(typeof(ServiceCollectionExtension))]
    [TestSubject(typeof(ServerSimulationAdapter))]
    public async Task EntityStateChangedNotifier_IsRegisteredAs_ScopedIActionSimulationProcessor(
        IServiceCollection collection)
    {
        var foundDescriptor = collection.FirstOrDefault(e =>
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