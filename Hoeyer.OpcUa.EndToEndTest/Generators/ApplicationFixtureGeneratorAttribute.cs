using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class ApplicationFixtureGeneratorAttribute<T> : DataSourceGeneratorAttribute<ApplicationFixture<T>>
    where T : notnull
{
    /// <inheritdoc />
    protected override IEnumerable<Func<ApplicationFixture<T>>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        //Is it possible to prepend all tests with the name of the protocol - sort of as a category
        var serviceMatches = FindMatchingServices();
        return new List<WebProtocol> { WebProtocol.OpcTcp }
            .SelectMany(protocol => ApplicationFixtureIterator(serviceMatches, protocol));
    }

    private static IEnumerable<Func<ApplicationFixture<T>>> ApplicationFixtureIterator(
        List<ServiceDescriptor> serviceMatches, WebProtocol protocol)
    {
        foreach (var service in serviceMatches)
        {
            var f = new ApplicationFixture<T>(service,
                new RunningSimulationServicesAttribute(protocol).ServiceCollection);
            yield return () => f;
        }
    }

    private static List<ServiceDescriptor> FindMatchingServices()
    {
        var allServices = new RunningSimulationServicesAttribute().ServiceCollection;
        var serviceMatches = new FilteredCollection(allServices, typeof(T))
            .Descriptors
            .DistinctBy(e => e.ImplementationType)
            .ToList();
        if (serviceMatches.Count == 0) throw new ArgumentException("There were no services to test!");
        return serviceMatches;
    }
}