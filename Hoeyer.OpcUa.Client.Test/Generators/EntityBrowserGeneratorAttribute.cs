using System.Diagnostics.CodeAnalysis;
using Hoeyer.Common.Reflection;
using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Reflection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.TestApplication;

namespace Hoeyer.OpcUa.ClientTest.Generators;
[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class EntityBrowserGeneratorAttribute : DataSourceGeneratorAttribute<OpcUaEntityBackendFixture<IEntityBrowser>>
{
    /// <inheritdoc />
    public override IEnumerable<Func<OpcUaEntityBackendFixture<IEntityBrowser>>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        var browserTypes = typeof(IEntityBrowser).Assembly
            .GetTypes()
            .First(t => typeof(IEntityBrowser).IsAssignableFrom(t) && t.IsAnnotatedWith<ClientServiceAttribute>());
        
        var entities = typeof(Gantry).Assembly
            .GetTypes()
            .Where(t => t.IsAnnotatedWith<OpcUaEntityAttribute>())
            .ToHashSet();

        return entities
            .Select(e => browserTypes.MakeGenericType(e))
            .SelectFunc(browserType => new OpcUaEntityBackendFixture<IEntityBrowser>(new OpcUaEntityTestApplication(), browserType));
    }
}