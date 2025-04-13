using System.Diagnostics.CodeAnalysis;
using Hoeyer.Common.Reflection;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.TestApplication;

namespace Hoeyer.OpcUa.Client.EndToEnd.Generators;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class
    ServiceImplementationFixtureAttribute<TWantedService> : DataSourceGeneratorAttribute<
    ApplicationFixture<TWantedService>>
    where TWantedService : notnull
{
    /// <inheritdoc />
    public override IEnumerable<Func<ApplicationFixture<TWantedService>>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        _ = typeof(Gantry).Assembly; //to include the necessary assembly
        var wanted = typeof(TWantedService);

        return OpcUaEntityServicesLoader
            .EntityServiceTypeContexts
            .Where(serviceContext => serviceContext.ConcreteServiceType.IsAssignableTo(wanted))
            .SelectFunc(e => new ApplicationFixture<TWantedService>(e.ConcreteServiceType));
    }
}