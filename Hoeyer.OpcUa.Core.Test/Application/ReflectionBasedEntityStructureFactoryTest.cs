using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructureFactory;
using Hoeyer.OpcUa.TestEntities;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Core.Test.Application;

[TestSubject(typeof(ReflectionBasedEntityStructureFactory<>))]
[TestSubject(typeof(IAgentStructureFactory<>))]
public sealed class ReflectionBasedEntityStructureFactoryTest
{
    private readonly Type _entityType = typeof(AllPropertyTypesEntity);
    private readonly ReflectionBasedEntityStructureFactory<AllPropertyTypesEntity> _testSubject = new();

    [Test]
    public async Task BaseObjectNameMatchesTypeName()
    {
        var result = _testSubject.Create(2).BaseObject;
        await Assert.That(result.BrowseName.Name).IsEqualTo(nameof(AllPropertyTypesEntity));
    }

    [Test]
    public async Task PropertyNamesMatch()
    {
        var nodePropertyNames = _testSubject.Create(2).PropertyByBrowseName.Keys;

        var subjectPropertyNames = _entityType.GetProperties().Select(e => e.Name).ToHashSet();
        using (Assert.Multiple())
        {
            foreach (var name in nodePropertyNames)
            {
                await Assert.That(subjectPropertyNames).Contains(name);
            }
        }
    }
}