using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Test.ServiceInjection;
using Hoeyer.OpcUa.Test.Simulation;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Test.Api;

public abstract class SimulationServiceContext<T>(Lazy<SimulationSetup> simulationSetup)
    : IAsyncDisposable, IAsyncInitializer
{
    private List<ISpecifiedTestSession<T>>? _specifiedSessions;
    protected IEnumerable<(ServiceDescriptor descriptor, Type entity)> ServiceDescriptors { get; private set; } = null!;
    protected IServiceProvider ServiceProvider => simulationSetup.Value.ServiceProvider;

    public async ValueTask DisposeAsync()
    {
        await simulationSetup.Value.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await simulationSetup.Value.InitializeAsync();
        ServiceDescriptors = new GenericEntityServiceDescriptionMatcher(
                typeof(T),
                simulationSetup.Value.ClientServices,
                ServiceProvider.GetService<EntityTypesCollection>()!
            )
            .GetMatchingDescriptors()
            .DistinctBy(e => e.ImplementationType)
            .Select(descriptor =>
                (descriptor, entity: descriptor.ImplementationType!.GenericTypeArguments[0]));
    }

    protected List<ISpecifiedTestSession<T>> GetSpecifiedSessions()
    {
        _specifiedSessions = ServiceDescriptors
            .Select(e => e.descriptor)
            .Select(CreateContext)
            .ToList();

        return _specifiedSessions;
    }

    private ISpecifiedTestSession<T> CreateContext(ServiceDescriptor descriptor)
    {
        var provider = ServiceProvider;
        var session = provider
            .GetRequiredService<IEntitySessionFactory>()
            .GetSessionFor(TestContext.Current!.Id);

        var service = (T)provider.GetRequiredService(descriptor.ServiceType);
        return new SpecifiedTestSession<T>(ServiceProvider, session, service);
    }
}