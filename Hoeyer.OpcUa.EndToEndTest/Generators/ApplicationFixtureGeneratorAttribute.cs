using Hoeyer.OpcUa.EndToEndTest.Fixtures;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class ApplicationFixtureGeneratorAttribute<T> : DataSourceGeneratorAttribute<ApplicationFixture<T>>
    where T : notnull
{
    /// <inheritdoc />
    protected override IEnumerable<Func<ApplicationFixture<T>>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        var allServices = new OpcFullSetupWithBackgroundServerFixtureAttribute().ServiceCollection;
        var serviceMatches = new FilteredCollection(allServices, typeof(T))
            .Descriptors
            .DistinctBy(e => e.ImplementationType)
            .ToList();
        if (serviceMatches.Count == 0) throw new ArgumentException("There were no services to test!");
        foreach (var service in serviceMatches)
        {
            var f = new ApplicationFixture<T>(service,
                new OpcFullSetupWithBackgroundServerFixtureAttribute().ServiceCollection);
            yield return () => f;
        }
    }
}