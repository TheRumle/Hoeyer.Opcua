using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Core.Test.Application;

[TestSubject(typeof(ReflectionBasedEntityStructureFactory<>))]
[TestSubject(typeof(IEntityNodeStructureFactory<>))]
[CoreServicesOfType<IEntityNodeStructureFactory>(typeof(ReflectionBasedEntityStructureFactory<>))]
public sealed class ReflectionBasedEntityStructureFactoryTest(
    GenericImplementation<IEntityNodeStructureFactory> fixture
)
{
    private readonly IBrowseNameCollection browseNames = fixture.BrowseNameCollection;
    private readonly IEntityNodeStructureFactory factory = fixture.Service;

    [Test]
    public async Task BaseObjectNameMatchesTypeName()
    {
        var result = factory.Create(2).BaseObject;
        await Assert.That(result.BrowseName.Name).IsEqualTo(fixture.BrowseNameCollection.EntityName);
    }

    public IEnumerable<Func<string>> CreatedProperties() => factory.Create(2).PropertyByBrowseName.Keys.SelectFunc();

    [Test]
    [InstanceMethodDataSource(nameof(CreatedProperties))]
    [DisplayName("Browse name '$propertyBrowseName' is the contained in browse name collection")]
    public async Task CreatedBrowseNamesMatchExpected(string propertyBrowseName)
    {
        var expectedBrowseNames = browseNames.PropertyNames.Values;
        await Assert.That(expectedBrowseNames).Contains(propertyBrowseName);
    }
}