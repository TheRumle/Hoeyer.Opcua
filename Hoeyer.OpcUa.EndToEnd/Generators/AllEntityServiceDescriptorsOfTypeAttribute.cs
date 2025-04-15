using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class AllEntityServiceDescriptorsOfTypeAttribute(params Type[] types) : DataSourceGeneratorAttribute<IReadOnlyCollection<ServiceDescriptor>> 
{
    /// <inheritdoc />
    public override IEnumerable<Func<IReadOnlyCollection<ServiceDescriptor>>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return types
            .SelectFunc(type => new ServiceDescriptorGenerator(type)
                .CreateServiceDescriptors()
                .ToList());
    }
}