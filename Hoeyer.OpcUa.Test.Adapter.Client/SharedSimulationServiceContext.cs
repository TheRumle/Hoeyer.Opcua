using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Test.ServiceInjection;
using Hoeyer.OpcUa.Test.Simulation;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Test.Adapter.Client;

public abstract class SharedSimulationServiceContext<T>(SimulationSetup simulationSetup)
    : IAsyncDisposable, IAsyncInitializer
{
    private List<ISpecifiedTestSession<T>>? _specifiedSessions;
    private IServiceProvider ServiceProvider => simulationSetup.ServiceProvider;

    public async ValueTask DisposeAsync()
    {
        await simulationSetup.DisposeAsync();
    }

    public async Task InitializeAsync() => await simulationSetup.InitializeAsync();

    public List<ISpecifiedTestSession<T>> GetSpecifiedSessions()
    {
        _specifiedSessions ??= DistinctServices()
            .Select(CreateContext)
            .Cast<ISpecifiedTestSession<T>>()
            .ToList();

        return _specifiedSessions;
    }

    private IEnumerable<ServiceDescriptor> DistinctServices()
    {
        return new ServiceDescriptionMatcher(
                typeof(T),
                simulationSetup.ClientServices,
                ServiceProvider.GetService<EntityTypesCollection>()!
            )
            .GetMatchingDescriptors()
            .DistinctBy(e => e.ImplementationType);
    }

    private SpecifiedTestSession<T> CreateContext(ServiceDescriptor descriptor)
    {
        var provider = ServiceProvider;
        var session = provider
            .GetRequiredService<IEntitySessionFactory>()
            .GetSessionFor(TestContext.Current!.Id);

        var service = (T)provider.GetRequiredService(descriptor.ServiceType);
        return new SpecifiedTestSession<T>(ServiceProvider, session, service);
    }
}