using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Test.Fixtures;
using Hoeyer.OpcUa.TestEntities;
using Hoeyer.OpcUa.TestEntities.Methods;
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
    [RegressionTest(
        "Translating Entities' List<string> properties to Lists<string> instead of string[] results in errors when trying to fetch the value from the server. The OpcUa framework cannot convert List<string> to a meaningful result when reading the node.",
        typeof(IEntityBrowser))]
    public async Task WhenTranslating_ToEntityNode_ListValuesAreTranslatedTo_Arrays(
        IEntityTranslator<Gantry> translator,
        IEntityNodeStructureFactory<Gantry> structure)
    {
        IEntityNode node = structure.Create(2);
        translator.AssignToNode(new Gantry
        {
            AAginList =
            [
                "stneriao",
                "tnserio"
            ],
            AList =
            [
                "These",
                " values failed before"
            ],
            IntValue = 21,
            StringValue = "hello",
            Position = Position.OverThere,
            HeldContainer = Guid.Empty,
            Occupied = false
        }, node);

        Func<string, object> propertyFor = name => node.PropertyByBrowseName[name].Value;
        using IDisposable assertion = Assert.Multiple();
        await Assert.That(propertyFor(nameof(Gantry.AAginList))).IsTypeOf<string[]>();
        await Assert.That(propertyFor(nameof(Gantry.AList))).IsTypeOf<string[]>();
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
            await Assert.That(entity.IntList).IsEquivalentTo((int[])node.PropertyByBrowseName["IntList"].Value);
        }
    }
}