using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Server.Test.TestData;

namespace Hoeyer.OpcUa.Server.Test.Fixtures.Application;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class
    ApplicationServiceCollectionGeneratorAttribute : DataSourceGeneratorAttribute<ApplicationServiceCollectionFixture>
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