using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Test.Fixtures;
using Hoeyer.opcUa.TestEntities;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Test.Application;

[GeneratedClassTest]
[TestSubject(typeof(IEntityTranslator<>))]
public class EntityTranslatorTest
{
    [Test]
    [ServiceCollectionDataSource]
    public async Task WhenAssigningValues_ToEntityNode_MethodsDoesNotDisappear(
        IEntityTranslator<AllPropertyTypesEntity> translator,
        IEntityNodeStructureFactory<AllPropertyTypesEntity> structure)
    {
        IEntityNode node = structure.Create(2);
        var entity = AllPropertyTypesEntity.CreateRandom();
        HashSet<MethodState> before = node.Methods.ToHashSet();

        translator.AssignToNode(entity, node);
        IEnumerable<MethodState> after = node.Methods;

        await Assert.That(before.SetEquals(after)).IsTrue();
    }

    [Test]
    [ServiceCollectionDataSource]
    public async Task WhenAssigningValues_ToEntityNode_PropertiesDoesNotDisappear(
        IEntityTranslator<AllPropertyTypesEntity> translator,
        IEntityNodeStructureFactory<AllPropertyTypesEntity> structure)
    {
        IEntityNode node = structure.Create(2);
        var entity = AllPropertyTypesEntity.CreateRandom();
        HashSet<PropertyState> before = node.PropertyStates.ToHashSet();
        translator.AssignToNode(entity, node);
        IEnumerable<PropertyState> after = node.PropertyStates;
        await Assert.That(before.SetEquals(after)).IsTrue();
    }


    [Test]
    [ServiceCollectionDataSource]
    public async Task WhenTranslating_MultipleTimes_StateIsPreserved(
        IEntityTranslator<AllPropertyTypesEntity> translator,
        IEntityNodeStructureFactory<AllPropertyTypesEntity> structure)
    {
        IEntityNode node = structure.Create(2);
        var entity = AllPropertyTypesEntity.CreateRandom();

        translator.AssignToNode(entity, node);
        await AssertPropertiesEqual(entity, node);

        AllPropertyTypesEntity newEntity = translator.Translate(node);
        translator.AssignToNode(newEntity, node);
        await AssertPropertiesEqual(entity, node);
    }


    [Test]
    [ServiceCollectionDataSource]
    public async Task When_AssigningToNode_ValuesAre_Equal(
        IEntityTranslator<AllPropertyTypesEntity> translator,
        IEntityNodeStructureFactory<AllPropertyTypesEntity> structure)
    {
        //Arrange
        IEntityNode node = structure.Create(2);
        var entity = AllPropertyTypesEntity.CreateRandom();

        //Act
        translator.AssignToNode(entity, node);

        //Assert
        await AssertPropertiesEqual(entity, node);
    }

    private static async Task AssertPropertiesEqual(AllPropertyTypesEntity entity, IEntityNode node)
    {
        using (Assert.Multiple())
        {
            await Assert.That((object)entity.Integer).IsEqualTo(node.PropertyByBrowseName["Integer"].Value);
            await Assert.That((object)entity.Long).IsEqualTo(node.PropertyByBrowseName["Long"].Value);
            await Assert.That((object)entity.String).IsEqualTo(node.PropertyByBrowseName["String"].Value);
            await Assert.That((object)entity.Guid).IsEqualTo(node.PropertyByBrowseName["Guid"].Value);
            await Assert.That((object)entity.Double).IsEqualTo(node.PropertyByBrowseName["Double"].Value);
            await Assert.That((object)entity.Float).IsEqualTo(node.PropertyByBrowseName["Float"].Value);
            await Assert.That((object)entity.Bool).IsEqualTo(node.PropertyByBrowseName["Bool"].Value);
            await Assert.That(entity.IntList).IsEquivalentTo((List<int>)node.PropertyByBrowseName["IntList"].Value);

            await Assert.That(entity.CustomIListMember)
                .IsEquivalentTo((ICollection<int>)node.PropertyByBrowseName["CustomIListMember"].Value);
        }
    }
}