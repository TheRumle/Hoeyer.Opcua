using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

public sealed class BrowseNameCollectionAttribute : DataSourceGeneratorAttribute<IBrowseNameCollection>
{
    /// <inheritdoc />
    protected override IEnumerable<Func<IBrowseNameCollection>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return OpcUaEntityTypes.Entities.SelectFunc(entityType => new EntityTypeModel(entityType));
    }
}