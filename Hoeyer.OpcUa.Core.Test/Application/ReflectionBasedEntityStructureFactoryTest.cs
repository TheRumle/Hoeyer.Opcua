using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using JetBrains.Annotations;
using Playground.Modelling.Models;

namespace Hoeyer.OpcUa.Core.Test.Application;

[TestSubject(typeof(ReflectionBasedEntityStructureFactory<>))]
[TestSubject(typeof(IEntityNodeStructureFactory<>))]
public sealed class ReflectionBasedEntityStructureFactoryTest
{
    private static readonly Type EntityType = typeof(AllPropertyTypesEntity);

    private static readonly ReflectionBasedEntityStructureFactory<AllPropertyTypesEntity> TestSubject =
        new(new EntityTypeModel<AllPropertyTypesEntity>(EntityType));

    [Test]
    public async Task BaseObjectNameMatchesTypeName()
    {
        var result = TestSubject.Create(2).BaseObject;
        await Assert.That(result.BrowseName.Name).IsEqualTo(nameof(AllPropertyTypesEntity));
    }

    [Test]
    public async Task PropertyNamesMatch()
    {
        var nodePropertyNames = TestSubject.Create(2).PropertyByBrowseName.Keys;

        var subjectPropertyNames = EntityType.GetProperties().Select(e => e.Name).ToHashSet();
        using (Assert.Multiple())
        {
            foreach (var name in nodePropertyNames)
            {
                await Assert.That(subjectPropertyNames).Contains(name);
            }
        }
    }
}