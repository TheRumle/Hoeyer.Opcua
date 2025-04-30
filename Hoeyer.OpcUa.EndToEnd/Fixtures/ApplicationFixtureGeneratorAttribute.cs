using Hoeyer.OpcUa.EndToEndTest.Generators;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public sealed class ApplicationFixtureGeneratorAttribute<T> : DataSourceGeneratorAttribute<ApplicationFixture<T>> where T : notnull
{
    /// <inheritdoc />
    public override IEnumerable<Func<ApplicationFixture<T>>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        var allServices = new AllOpcUaServicesFixture().ServiceCollection;
        var serviceMatches = new FilteredCollection(allServices, typeof(T))
            .Descriptors
            .DistinctBy(e => e.ImplementationType)
            .ToList();
        
        foreach (var service in serviceMatches)
        {
            var f = new ApplicationFixture<T>(service, allServices);
            f.InitializeAsync().Wait();
            yield return () => f;
        }
    }
}