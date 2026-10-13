using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Fixtures.Common.TUnit.Configuration.Assertions;
using Hoeyer.OpcUa.Fixtures.Core.Fixtures;
using Hoeyer.OpcUa.Fixtures.Core.Fixtures.TestEntities;
using JetBrains.Annotations;
using Opc.Ua;

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
            EnumVal = AllPropertyTypesEntity.EnumValueProp.start,
            Guid = Guid.NewGuid(),
            Bool = false,
            StringList = ["five hundred", "Cigarettes"]
        }, node);

        List<TTarget> PropertyFor<TTarget>(string name) => node.PropertyByBrowseName[name].Value.CastTo<ArrayOf<TTarget>>().ToList();
        using var assertion = Assert.Multiple();
        await Assert.That(PropertyFor<int>(nameof(AllPropertyTypesEntity.IntList))).IsTypeOf<List<int>>();
        await Assert.That(PropertyFor<string>(nameof(AllPropertyTypesEntity.StringList))).IsTypeOf<List<string>>();
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

        await Assert.That(node.PropertyByBrowseName[nameof(AllPropertyTypesEntity.Integer)].Value).VariantEquals(entity.Integer);
        await Assert.That(node.PropertyByBrowseName[nameof(AllPropertyTypesEntity.Integer)].Value).VariantEquals(entity.Integer);
        await Assert.That(node.PropertyByBrowseName[nameof(AllPropertyTypesEntity.Long)].Value).VariantEquals(entity.Long);
        await Assert.That(node.PropertyByBrowseName[nameof(AllPropertyTypesEntity.String)].Value).VariantEquals(entity.String);
        await Assert.That(node.PropertyByBrowseName[nameof(AllPropertyTypesEntity.Double)].Value).VariantEquals(entity.Double);
        await Assert.That(node.PropertyByBrowseName[nameof(AllPropertyTypesEntity.Float)].Value).VariantEquals(entity.Float);
        await Assert.That(node.PropertyByBrowseName[nameof(AllPropertyTypesEntity.Bool)].Value).VariantEquals(entity.Bool);
    }
}