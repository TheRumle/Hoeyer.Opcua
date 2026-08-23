using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Test.Fixtures;
using Hoeyer.OpcUa.Core.Test.Fixtures.TestEntities;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Core.Test.Application;

[TestSubject(typeof(IEntityTranslator<>))]
public class EntityTranslatorTest
{
    [Test]
    [CoreServiceInjection]
    public async Task WhenAssigningValues_ToEntityNode_MethodsDoesNotDisappear(
        IEntityTranslator<AllPropertyTypesEntity> translator,
        IEntityNodeStructureFactory<AllPropertyTypesEntity> structure)
    {
        var node = structure.Create(2);
        var entity = AllPropertyTypesEntity.CreateRandom();
        var before = node.Methods.ToHashSet();

        translator.AssignToNode(entity, node);
        var after = node.Methods;

        await Assert.That(before.SetEquals(after)).IsTrue();
    }

    [Test]
    [CoreServiceInjection]
    public async Task WhenAssigningValues_ToEntityNode_PropertiesDoesNotDisappear(
        IEntityTranslator<AllPropertyTypesEntity> translator,
        IEntityNodeStructureFactory<AllPropertyTypesEntity> structure)
    {
        var node = structure.Create(2);
        var entity = AllPropertyTypesEntity.CreateRandom();
        var before = node.PropertyStates.ToHashSet();
        translator.AssignToNode(entity, node);
        var after = node.PropertyStates;
        await Assert.That(before.SetEquals(after)).IsTrue();
    }

    [Test]
    [CoreServiceInjection]
    public async Task WhenTranslating_ToEntityNode_ListValuesAreTranslatedTo_Arrays(
        IEntityTranslator<AllPropertyTypesEntity> translator,
        IEntityNodeStructureFactory<AllPropertyTypesEntity> structure)
    {
        var node = structure.Create(2);
        translator.AssignToNode(new AllPropertyTypesEntity
        {
            IntList =
            [
                123, 321
            ],
            Integer = 321312,
            String = "hello",
            EnumVal = AllPropertyTypesEntity.EnumValue.start,
            Guid = Guid.NewGuid(),
            Bool = false,
            StringList = ["five hundred", "Cigarettes"]
        }, node);

        Func<string, object> propertyFor = name => node.PropertyByBrowseName[name].Value;
        using var assertion = Assert.Multiple();
        await Assert.That(propertyFor(nameof(AllPropertyTypesEntity.IntList))).IsTypeOf<int[]>();
        await Assert.That(propertyFor(nameof(AllPropertyTypesEntity.StringList))).IsTypeOf<string[]>();
    }


    [Test]
    [CoreServiceInjection]
    public async Task WhenTranslating_MultipleTimes_StateIsPreserved(
        IEntityTranslator<AllPropertyTypesEntity> translator,
        IEntityNodeStructureFactory<AllPropertyTypesEntity> structure)
    {
        var node = structure.Create(2);
        var entity = AllPropertyTypesEntity.CreateRandom();

        translator.AssignToNode(entity, node);
        await AssertPropertiesEqual(entity, node);

        var newEntity = translator.Translate(node);
        translator.AssignToNode(newEntity, node);
        await AssertPropertiesEqual(entity, node);
    }


    [Test]
    [CoreServiceInjection]
    public async Task When_AssigningToNode_ValuesAre_Equal(
        IEntityTranslator<AllPropertyTypesEntity> translator,
        IEntityNodeStructureFactory<AllPropertyTypesEntity> structure)
    {
        //Arrange
        var node = structure.Create(2);
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
            await Assert.That(entity.Integer).IsEquivalentTo(node.PropertyByBrowseName["Integer"].Value);
            await Assert.That(entity.Long).IsEquivalentTo(node.PropertyByBrowseName["Long"].Value);
            await Assert.That(entity.String).IsEquivalentTo(node.PropertyByBrowseName["String"].Value);
            await Assert.That(entity.Guid).IsEquivalentTo(node.PropertyByBrowseName["Guid"].Value);
            await Assert.That(entity.Double).IsEquivalentTo(node.PropertyByBrowseName["Double"].Value);
            await Assert.That(entity.Float).IsEquivalentTo(node.PropertyByBrowseName["Float"].Value);
            await Assert.That(entity.Bool).IsEquivalentTo(node.PropertyByBrowseName["Bool"].Value);
            await Assert.That(entity.IntList).IsEquivalentTo((int[])node.PropertyByBrowseName["IntList"].Value);
        }
    }
}