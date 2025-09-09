using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;
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
public sealed class ServiceExtensionTest()
    : ServiceInjectionTest(new ServiceCollectionFixture().SimulationServices, TestData)
{
    public static IEnumerable<IPartialServiceMatcher> TestData =>
    [
        new GenericMatcher(typeof(ISubscriptionManager<>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(IMessageSubscriptionFactory<>), ServiceLifetime.Singleton),
        new ConcreteServiceWithGenericImplMatcher<ISimulationStepValidator>(
            typeof(ReturnValueOrderValidator<,,>), ServiceLifetime.Transient),
        new GenericMatcher(typeof(ISimulationExecutor<,,>), ServiceLifetime.Transient),
        new GenericMatcher(typeof(ISimulationExecutor<,>), ServiceLifetime.Transient),
        new GenericMatcher(typeof(ISimulationOrchestrator<,,>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(ISimulationOrchestrator<,>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(ISimulationProcessorPipeline<,,>), ServiceLifetime.Singleton),
        new GenericMatcher(typeof(ISimulationProcessorPipeline<,>), ServiceLifetime.Singleton)
    ];
}