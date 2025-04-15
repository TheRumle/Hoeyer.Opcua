using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class EntityServiceDescriptorsOfTypeAttribute(Type type) : DataSourceGeneratorAttribute<ServiceDescriptor>
{
    public override IEnumerable<Func<ServiceDescriptor>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return new ServiceDescriptorGenerator(type).CreateServiceDescriptors().SelectFunc();
    }
}