using System.Reflection;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Hoeyer.OpcUa.TestEntities;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.EndToEndTest;

[TestSubject(typeof(IAgentBrowser<>))]
[TestSubject(typeof(INodeReader))]
public sealed class AgentBrowserTest
{
    [Test]
    [ApplicationFixtureGenerator<IAgentBrowser>]
    public async Task AgentBrowser_CanCreateAgent_AndTranslateIt(ApplicationFixture<IAgentBrowser> services)
    {
        var agent = await services.ExecuteAsync(browser => browser.BrowseAgent(CancellationToken.None));

        await Assert.That(agent).IsNotDefault();
        await Assert.That(agent.PropertyStates).IsNotEmpty();
        await Assert.That(agent.BaseObject).IsNotDefault();
    }

    [Test]
    [ApplicationFixtureGenerator<IAgentBrowser>]
    public async Task AgentBrowser_BrowsedAgent_DoesNotHaveNullValues(ApplicationFixture<IAgentBrowser> services)
    {
        var agent = await services.ExecuteAsync(browser => browser.BrowseAgent(CancellationToken.None));
        Dictionary<string, object?> propertyValue = GetNullPropertyValues(agent);

        using (Assert.Multiple())
        {
            foreach (var key in propertyValue.Keys)
            {
                Assert.Fail(
                    $"{agent.BaseObject.BrowseName.Name}.{key} was null and no browsed property should be null");
            }
        }
    }

    [Test]
    [TestSubject(typeof(IAgentTranslator<Gantry>))]
    [ClassDataSource<ApplicationFixture>]
    public async Task AgentBrowser_BrowsedAgent_CanBeTranslated_DoesNotHaveNullValues(ApplicationFixture services)
    {
        var browser = services.GetService<IAgentBrowser<Gantry>>();
        var translator = services.GetService<IAgentTranslator<Gantry>>();
        var node = await browser.BrowseAgent();
        var gantry = translator.Translate(node);

        var propsWithNullValue = gantry
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => (property.Name, Value: property.GetValue(gantry)))
            .Where(e => e.Value == null);

        await Assert
            .That(propsWithNullValue)
            .IsEmpty()
            .Because(" after reading the node and translating it, there should not be null values");
    }

    [ApplicationFixtureGenerator<IAgentBrowser>]
    [Test]
    public void CanBrowseOnSimultaneousThreads(ApplicationFixture<IAgentBrowser> fixture)
    {
        IAgentBrowser browser = fixture.TestedService;
        var first = new Thread(() => browser.BrowseAgent(CancellationToken.None).GetAwaiter().GetResult());
        var second = new Thread(() => browser.BrowseAgent(CancellationToken.None).GetAwaiter().GetResult());
        first.Start();
        second.Start();
        first.Join();
        second.Join();
    }

    private static Dictionary<string, object?> GetNullPropertyValues(IAgent agent)
    {
        return agent.PropertyStates
            .Where(e => e.Value is null or Variant { Value: null })
            .ToDictionary<PropertyState, string, object>(prop => prop.BrowseName.Name, prop => null!)!;
    }
}