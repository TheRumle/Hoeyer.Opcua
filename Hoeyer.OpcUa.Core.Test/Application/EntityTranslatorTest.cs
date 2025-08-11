using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Test.Fixtures;
using Hoeyer.OpcUa.TestEntities;
using Hoeyer.OpcUa.TestEntities.Methods;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Test.Application;

[GeneratedClassTest]
[TestSubject(typeof(IAgentTranslator<>))]
public class AgentTranslatorTest
{
    [Test]
    [ServiceCollectionDataSource]
    public async Task WhenAssigningValues_ToAgent_MethodsDoesNotDisappear(
        IAgentTranslator<AllPropertyTypesAgent> translator,
        IAgentStructureFactory<AllPropertyTypesAgent> structure)
    {
        var node = structure.Create(2);
        var agent = AllPropertyTypesAgent.CreateRandom();
        HashSet<MethodState> before = node.Methods.ToHashSet();

        translator.AssignToNode(agent, node);
        IEnumerable<MethodState> after = node.Methods;

        await Assert.That(before.SetEquals(after)).IsTrue();
    }

    [Test]
    [ServiceCollectionDataSource]
    public async Task WhenAssigningValues_ToAgent_PropertiesDoesNotDisappear(
        IAgentTranslator<AllPropertyTypesAgent> translator,
        IAgentStructureFactory<AllPropertyTypesAgent> structure)
    {
        var node = structure.Create(2);
        var agent = AllPropertyTypesAgent.CreateRandom();
        HashSet<PropertyState> before = node.PropertyStates.ToHashSet();
        translator.AssignToNode(agent, node);
        IEnumerable<PropertyState> after = node.PropertyStates;
        await Assert.That(before.SetEquals(after)).IsTrue();
    }

    [Test]
    [ServiceCollectionDataSource]
    public async Task WhenTranslating_ToAgent_ListValuesAreTranslatedTo_Arrays(
        IAgentTranslator<Gantry> translator,
        IAgentStructureFactory<Gantry> structure)
    {
        var node = structure.Create(2);
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
        IAgentTranslator<AllPropertyTypesAgent> translator,
        IAgentStructureFactory<AllPropertyTypesAgent> structure)
    {
        var node = structure.Create(2);
        var agent = AllPropertyTypesAgent.CreateRandom();

        translator.AssignToNode(agent, node);
        await AssertPropertiesEqual(agent, node);

        AllPropertyTypesAgent newAgent = translator.Translate(node);
        translator.AssignToNode(newAgent, node);
        await AssertPropertiesEqual(agent, node);
    }


    [Test]
    [ServiceCollectionDataSource]
    public async Task When_AssigningToNode_ValuesAre_Equal(
        IAgentTranslator<AllPropertyTypesAgent> translator,
        IAgentStructureFactory<AllPropertyTypesAgent> structure)
    {
        //Arrange
        var node = structure.Create(2);
        var agent = AllPropertyTypesAgent.CreateRandom();

        //Act
        translator.AssignToNode(agent, node);

        //Assert
        await AssertPropertiesEqual(agent, node);
    }

    private static async Task AssertPropertiesEqual(AllPropertyTypesAgent agent, IAgent node)
    {
        using (Assert.Multiple())
        {
            await Assert.That((object)agent.Integer).IsEqualTo(node.PropertyByBrowseName["Integer"].Value);
            await Assert.That((object)agent.Long).IsEqualTo(node.PropertyByBrowseName["Long"].Value);
            await Assert.That((object)agent.String).IsEqualTo(node.PropertyByBrowseName["String"].Value);
            await Assert.That((object)agent.Guid).IsEqualTo(node.PropertyByBrowseName["Guid"].Value);
            await Assert.That((object)agent.Double).IsEqualTo(node.PropertyByBrowseName["Double"].Value);
            await Assert.That((object)agent.Float).IsEqualTo(node.PropertyByBrowseName["Float"].Value);
            await Assert.That((object)agent.Bool).IsEqualTo(node.PropertyByBrowseName["Bool"].Value);
            await Assert.That(agent.IntList).IsEquivalentTo((int[])node.PropertyByBrowseName["IntList"].Value);
        }
    }
}