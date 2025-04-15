using Hoeyer.Opc.Ua.Test.TUnit.Extensions;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class SingleServiceApplicationTestGeneratorAttribute<TInterfaceOfWanted>(Type wantedService) : DataSourceGeneratorAttribute<SingleServiceTestFixture<TInterfaceOfWanted>> where TInterfaceOfWanted : notnull
{
    private readonly EntityServiceDescriptorsOfTypeAttribute _serviceDescriptorGenerator = new(wantedService);

    /// <inheritdoc />
    public override IEnumerable<Func<SingleServiceTestFixture<TInterfaceOfWanted>>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return _serviceDescriptorGenerator.GenerateDataSources(dataGeneratorMetadata)
            .Map(descriptor => 
                new SingleServiceTestFixture<TInterfaceOfWanted>(
                    applicationFixture => applicationFixture.GetService<TInterfaceOfWanted>(descriptor.ServiceType)));
    }
}