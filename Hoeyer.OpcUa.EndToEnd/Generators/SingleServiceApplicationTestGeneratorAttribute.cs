using Hoeyer.Opc.Ua.Test.TUnit.Extensions;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class SingleServiceApplicationTestGeneratorAttribute<TInterfaceOfWanted>(Type wantedService) : DataSourceGeneratorAttribute<ServiceFixture<TInterfaceOfWanted>> where TInterfaceOfWanted : notnull
{
    private readonly EntityServiceDescriptorsOfTypeAttribute _serviceDescriptorGenerator = new(wantedService);

    /// <inheritdoc />
    public override IEnumerable<Func<ServiceFixture<TInterfaceOfWanted>>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return _serviceDescriptorGenerator.GenerateDataSources(dataGeneratorMetadata)
            .Map(descriptor => 
                new ServiceFixture<TInterfaceOfWanted>(
                    applicationFixture => applicationFixture.GetService<TInterfaceOfWanted>(descriptor.ServiceType)));
    }
}