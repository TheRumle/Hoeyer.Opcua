using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Server.Test.Fixtures.Application.NodeServices;
using Hoeyer.OpcUa.Server.Test.TestData;

namespace Hoeyer.OpcUa.Server.Test.Generators;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class ApplicationServiceCollectionGeneratorAttribute : DataSourceGeneratorAttribute<ApplicationServiceCollectionFixture>
{
    /// <inheritdoc />
    public override IEnumerable<Func<ApplicationServiceCollectionFixture>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return GeneratedTypes
            .EntityNodeCreators
            .Select(creator =>
                (Func<ApplicationServiceCollectionFixture>)(() => new ApplicationServiceCollectionFixture(creator)));
    }
}


[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class EntityPropertyFixtureGeneratorAttribute : DataSourceGeneratorAttribute<EntityPropertyFixture>
{
  
    
    /// <inheritdoc />
    public override IEnumerable<Func<EntityPropertyFixture>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return from serviceCollection in CreateServiceCollection(dataGeneratorMetadata)
            from property in serviceCollection.PropertyStates 
            select (Func<EntityPropertyFixture>)(() => new EntityPropertyFixture(property.Value, serviceCollection.EntityName));
    }

    private static IEnumerable<ApplicationServiceCollectionFixture> CreateServiceCollection(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return new ApplicationServiceCollectionGeneratorAttribute()
            .GenerateDataSources(dataGeneratorMetadata)
            .Select(e => e.Invoke());
    }
}
