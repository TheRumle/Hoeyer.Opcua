using System.Diagnostics.CodeAnalysis;
using Hoeyer.Common.Reflection;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.TestApplication;

namespace Hoeyer.OpcUa.Test.Client.EndToEnd.Generators;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class ApplicationFixtureGeneratorAttribute<TWantedClientService> : DataSourceGeneratorAttribute<ApplicationFixture<TWantedClientService>> 
    where TWantedClientService : notnull
{
    /// <inheritdoc />
    public override IEnumerable<Func<ApplicationFixture<TWantedClientService>>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        var clientServices = typeof(IEntityBrowser).Assembly
            .GetTypes()
            .First(t => typeof(TWantedClientService).IsAssignableFrom(t) && t is { IsClass: true, IsInterface: false });
        
        var entities = typeof(Gantry).Assembly
            .GetTypes()
            .Where(t => t.IsAnnotatedWith<OpcUaEntityAttribute>())
            .ToHashSet();
        
        return entities
            .Select(e => clientServices.MakeGenericType(e))
            .SelectFunc(browserType => new ApplicationFixture<TWantedClientService>(browserType));
    }
}