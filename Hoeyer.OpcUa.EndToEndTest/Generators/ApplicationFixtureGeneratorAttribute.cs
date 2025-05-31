using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class ApplicationFixtureGeneratorAttribute<T> : DataSourceGeneratorAttribute<ApplicationFixture<T>>
    where T : notnull
{
    /// <inheritdoc />
    public override IEnumerable<Func<ApplicationFixture<T>>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        IServiceCollection allServices = new OpcFullSetupWithBackgroundServerFixture().ServiceCollection;
        var serviceMatches = new FilteredCollection(allServices, typeof(T))
            .Descriptors
            .DistinctBy(e => e.ImplementationType)
            .ToList();
        if (serviceMatches.Count == 0) throw new ArgumentException("There were no services to test!");
        return ApplicationFixtureIterator(dataGeneratorMetadata, serviceMatches);
    }

    private static IEnumerable<Func<ApplicationFixture<T>>> ApplicationFixtureIterator(
        DataGeneratorMetadata dataGeneratorMetadata, List<ServiceDescriptor> serviceMatches)
    {
        foreach (var service in serviceMatches)
        {
            var f = new ApplicationFixture<T>(service, new OpcFullSetupWithBackgroundServerFixture().ServiceCollection);
            dataGeneratorMetadata.TestBuilderContext.Current.Events.OnTestRegistered += async (_, _) =>
            {
                await f.InitializeAsync();
            };
            yield return () => f;
        }
    }
}