using System.Collections;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Test.Adapter;
using Hoeyer.OpcUa.Test.ServiceInjection;
using Hoeyer.OpcUa.Test.Simulation;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Test.Api;

public class SimulationServiceContext<T>(SimulationSetup simulationSetup)
    : ISimulationServiceContext<T>
{
    public SimulationServiceContext(ITestFrameworkAdapter simulationSetup) : this(new SimulationSetup(simulationSetup))
    {
    }

    private IEnumerable<(ServiceDescriptor descriptor, Type entity)> ServiceDescriptors { get; set; } = null!;
    public SimulationSetup SimulationSetup => simulationSetup;
    public IServiceProvider ServiceProvider => simulationSetup.ServiceProvider;

    public async ValueTask DisposeAsync()
    {
        await simulationSetup.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await simulationSetup.InitializeAsync();
        ServiceDescriptors = new GenericEntityServiceDescriptionMatcher(
                typeof(T),
                simulationSetup.ClientServices,
                ServiceProvider.GetService<EntityTypesCollection>()!
            )
            .GetMatchingDescriptors()
            .DistinctBy(e => e.ImplementationType)
            .Select(descriptor =>
                (descriptor, entity: descriptor.ImplementationType!.GenericTypeArguments[0]));
    }

    public List<ISimulationTestContext<T>> GetSimulationSession() =>
        ServiceDescriptors
            .Select(e => e.descriptor)
            .Select(CreateContext)
            .ToList();

    private ISimulationTestContext<T> CreateContext(ServiceDescriptor descriptor)
    {
        var provider = ServiceProvider;
        var session = provider
            .GetRequiredService<IEntitySessionFactory>()
            .GetSessionFor(SimulationSetup.SimulationTestIdentity.ToString());

        var service = (T)provider.GetRequiredService(descriptor.ServiceType);
        return new SimulationTestContext<T>(ServiceProvider, session, service);
    }

    public static implicit operator SimulationTestServiceList<T>(SimulationServiceContext<T> context) => new(context);
}

public class SimulationTestServiceList<T>(SimulationServiceContext<T> context)
    : IEnumerable<ISimulationTestContext<T>>, IAsyncDisposable
{
    private readonly IAsyncDisposable _session = context;

    public IReadOnlyList<ISimulationTestContext<T>> Elements { get; } = context.GetSimulationSession();

    public async ValueTask DisposeAsync() => await _session.DisposeAsync();
    public IEnumerator<ISimulationTestContext<T>> GetEnumerator() => Elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Elements).GetEnumerator();
}