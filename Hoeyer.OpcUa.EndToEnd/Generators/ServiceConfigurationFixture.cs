using System.Diagnostics.CodeAnalysis;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class
    ServiceConfigurationFixture(Type wantedService) : DataSourceGeneratorAttribute<ServiceDescriptor>
{
    IEnumerable<ServiceDescriptor> _serviceCollection = new OpcUaCoreServicesFixture()
        .ServiceCollection.Union(new OpcUaServerServiceFixture().ServiceCollection)
        .Union(new OpcUaClientServicesFixture().ServiceCollection);
    
    /// <inheritdoc />
    public override IEnumerable<Func<ServiceDescriptor>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (wantedService.IsGenericTypeDefinition)
        {
            return _serviceCollection
                .Where(e=>e.ServiceType.IsGenericTypeDefinition && e.ServiceType.GetGenericTypeDefinition() == wantedService)
                .SelectFunc();
        }
        
        return _serviceCollection
            .Where(e=>e.ServiceType.IsAssignableFrom(wantedService))
            .SelectFunc();
    }
}