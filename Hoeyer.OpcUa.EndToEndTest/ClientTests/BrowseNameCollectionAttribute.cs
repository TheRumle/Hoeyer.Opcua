using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

public sealed class BrowseNameCollectionAttribute : DataSourceGeneratorAttribute<BrowseNameCollection>
{
    /// <inheritdoc />
    protected override IEnumerable<Func<BrowseNameCollection>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return OpcUaEntityTypes.Entities.SelectFunc(entityType => new BrowseNameCollection(entityType));
    }
}