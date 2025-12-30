using System.Collections;
using Hoeyer.Common.Extensions.Collection;
using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;

public class SimulationFixture : IAsyncInitializer, IAsyncDisposable
{
    private IEntitySessionFactory? _sessionFactory;
    protected SimulationFixtureContext Context = null!;

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await Context.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        Context = new SimulationFixtureContext();
        await Context.InitializeAsync();
    }

    public T GetService<T>() where T : notnull => Context.ServiceProvider.GetService<T>()!;

    public T GetService<T>(Type t) where T : notnull => (T)Context.ServiceProvider.GetRequiredService(t);

    public async Task<ISession> CreateSession()
    {
        _sessionFactory ??= Context.ServiceProvider.GetRequiredService<IEntitySessionFactory>();
        return (await _sessionFactory.GetSessionForAsync(TestContext.Current!.Id!)).Session;
    }

    public async Task ExecuteActionAsync(Func<ISession, Task> action) =>
        await action.Invoke(await CreateSession());

    public async Task<T> ExecuteFunctionAsync<T>(Func<ISession, Task<T>> action) =>
        await action.Invoke(await CreateSession());
}

public sealed class SimulationFixture<T> : SimulationFixture, IEnumerable<Func<SimulationContext<T>>>
{
    private List<SimulationContext<T>>? _servicesUnderTestProvider;

    private Lazy<ServiceDescriptionMatcher> DescriptionMatcher =>
        new(() => new ServiceDescriptionMatcher(
            typeof(T),
            Context.ServerDependentServices.Services,
            Context.ServiceProvider.GetService<EntityTypesCollection>())
        );

    public IEnumerator<Func<SimulationContext<T>>> GetEnumerator() =>
        GetServicesUnderTest().SelectFunc().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IReadOnlyList<SimulationContext<T>> GetServicesUnderTest()
    {
        _servicesUnderTestProvider ??= DescriptionMatcher
            .Value
            .GetMatchingDescriptors()
            .DistinctBy(e => e.ImplementationType)
            .Select(CreateFixture)
            .ToList();
        return _servicesUnderTestProvider;
    }

    private SimulationContext<T> CreateFixture(ServiceDescriptor descriptor)
    {
        var provider = Context.ServiceProvider;
        var session = provider
            .GetRequiredService<IEntitySessionFactory>()
            .GetSessionFor(TestContext.Current!.Id);

        var service = (T)provider
            .GetRequiredService(descriptor.ServiceType)!;
        return new SimulationContext<T>(Context.ServiceProvider, session, service);
    }
}