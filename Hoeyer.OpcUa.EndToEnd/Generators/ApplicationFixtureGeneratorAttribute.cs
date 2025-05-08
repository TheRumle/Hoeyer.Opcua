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

        if (serviceMatches.Count == 0) throw new ArgumentException("There were no services to test!");
        foreach (var service in serviceMatches)
        {
            var f = new ApplicationFixture<T>(service, allServices);
            f.InitializeAsync().Wait();
            yield return () => f;
        }
    }
}