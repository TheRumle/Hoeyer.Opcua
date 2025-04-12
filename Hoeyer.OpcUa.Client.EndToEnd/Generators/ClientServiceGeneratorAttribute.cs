using System.Diagnostics.CodeAnalysis;
using Hoeyer.Common.Reflection;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.TestApplication;

namespace Hoeyer.OpcUa.ClientTest.Generators;
[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class ClientServiceGeneratorAttribute<TWantedClientService> : DataSourceGeneratorAttribute<ClientFixture<TWantedClientService>> 
    where TWantedClientService : notnull
{
    /// <inheritdoc />
    public override IEnumerable<Func<ClientFixture<TWantedClientService>>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
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
            .SelectFunc(browserType => new ClientFixture<TWantedClientService>(new OpcUaEntityTestApplication(), browserType));
    }
}