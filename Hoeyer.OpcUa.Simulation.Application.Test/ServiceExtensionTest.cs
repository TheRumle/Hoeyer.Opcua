using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.Execution;
using Hoeyer.OpcUa.Simulation.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Simulation.Application.Test.Fixtures;

namespace Simulation.Application.Test;

[TestSubject(typeof(ServiceCollectionExtension))]
[TestSubject(typeof(SimulationServicesContainer))]
[InheritsTests]
[ClassDataSource<ServiceCollectionFixture>]
public sealed class
    ServiceExtensionTest(
        ServiceCollectionFixture
            fixture) //: ServiceInjectionTest(new ServiceCollectionFixture().SimulationServices, AllMatchers)
{
    private static readonly Dictionary<ServiceLifetime, List<IPartialServiceMatcher>> AllMatchersByLifetime =
        CreateLifetimeDictionary();

    private readonly IServiceCollection _collection = fixture.SimulationServices;

    public static IEnumerable<IPartialServiceMatcher> AllMatchers() =>
    [
        new GenericMatcher(typeof(ISubscriptionManager<>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(IMessageSubscriptionFactory<>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(ISimulationBuilderFactory<,>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(ISimulationBuilderFactory<,,>), ServiceLifetime.Singleton),
        new ConcreteServiceWithGenericImplMatcher<ISimulationStepValidator>(
            typeof(ReturnValueOrderValidator<,,>), ServiceLifetime.Transient),
        new GenericMatcher(typeof(ISimulationExecutor<,,>), ServiceLifetime.Transient),
        new GenericMatcher(typeof(ISimulationExecutor<,>), ServiceLifetime.Transient),
        new GenericMatcher(typeof(ISimulationOrchestrator<,,>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(ISimulationOrchestrator<,>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(ISimulationProcessorPipeline<,,>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(ISimulationProcessorPipeline<,>), ServiceLifetime.Singleton)
    ];

    private static Dictionary<ServiceLifetime, List<IPartialServiceMatcher>> CreateLifetimeDictionary()
    {
        Dictionary<ServiceLifetime, List<IPartialServiceMatcher>> result = new();
        foreach (var matcher in AllMatchers())
        {
            var collection = result.GetOrAdd(matcher.Lifetime, []);
            collection.Add(matcher);
        }

        return result;
    }

    private static List<IPartialServiceMatcher> MatchersWithLifetime(ServiceLifetime lifetime)
        => AllMatchersByLifetime[lifetime];

    private IEnumerable<ServiceDescriptor> GetMatchingDescriptors(IEnumerable<IPartialServiceMatcher> matchers) =>
        from descriptor in _collection
        from matcher in matchers
        where matcher.Equals(descriptor)
        select descriptor;

    public IEnumerable<ServiceDescriptor> SingletonServices() =>
        GetMatchingDescriptors(MatchersWithLifetime(ServiceLifetime.Singleton));

    public IEnumerable<ServiceDescriptor> TransientServices() =>
        GetMatchingDescriptors(MatchersWithLifetime(ServiceLifetime.Transient));

    public IEnumerable<ServiceDescriptor> ScopedServices() =>
        GetMatchingDescriptors(MatchersWithLifetime(ServiceLifetime.Scoped));


    [Test]
    [DisplayName("$descriptor can be provided as singleton.")]
    [InstanceMethodDataSource<ServiceExtensionTest>(nameof(SingletonServices))]
    public async Task ServiceCanBeCreatedAsSingleton(ServiceDescriptor descriptor) =>
        await AssertServiceCanBeCreated(descriptor);

    [Test]
    [DisplayName("$descriptor can be provided as singleton.")]
    [InstanceMethodDataSource<ServiceExtensionTest>(nameof(TransientServices))]
    public async Task ServiceCanBeCreatedAsTransient(ServiceDescriptor descriptor) =>
        await AssertServiceCanBeCreated(descriptor);

    [Test]
    [DisplayName("$descriptor can be provided as scoped.")]
    [InstanceMethodDataSource<ServiceExtensionTest>(nameof(ScopedServices))]
    public async Task ServiceCanBeCreatedAsScoped(ServiceDescriptor descriptor) =>
        await AssertServiceCanBeCreatedScoped(descriptor);


    [Test]
    [DisplayName("$matcher has been registered")]
    [InstanceMethodDataSource<ServiceExtensionTest>(nameof(AllMatchers))]
    public async Task ServiceIsRegistered(IPartialServiceMatcher matcher)
    {
        await Assert.That(_collection.Any(matcher.Equals)).IsTrue();
    }


    private async Task AssertServiceCanBeCreated(ServiceDescriptor descriptor)
    {
        var provider = _collection.BuildServiceProvider();
        var foundService = provider.GetService(descriptor.ServiceType);
        await Assert.That(foundService).IsNotNull().Because("We want to construct the service as a singleton");
    }

    private async Task AssertServiceCanBeCreatedScoped(ServiceDescriptor descriptor)
    {
        var provider = _collection.BuildServiceProvider().CreateAsyncScope();
        var foundService = provider.ServiceProvider.GetService(descriptor.ServiceType);
        await Assert.That(foundService).IsNotNull().Because("We want to construct the service as a singleton");
    }

    private class TestFactory<T> : IMessageSubscriptionFactory<T>
    {
        /// <inheritdoc />
        public IMessageSubscription<T> CreateSubscription(IMessageConsumer<T> consumer,
            Action<IMessageSubscription<T>>? disposeCallBack = null) => null!;
    }
}