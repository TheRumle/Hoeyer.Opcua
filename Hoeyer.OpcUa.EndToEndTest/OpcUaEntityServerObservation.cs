using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.opcUa.TestEntities;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.EndToEndTest;

[ClassDataSource<ApplicationFixture>]
public sealed class OpcUaEntityServerObservation(ApplicationFixture fixture)
{
    [Test]
    public async Task WhenClientWritesToEntity_ObserverShouldBeNotified()
    {
        var session = await fixture.CreateSession(Guid.NewGuid().ToString());
        IEntityChangedBroadcaster<Gantry> publisher = fixture.GetService<IEntityChangedBroadcaster<Gantry>>();
        var observer = new TestSubscriber<Gantry>();
        _ = publisher.EntitySubscriptionManager.Subscribe(observer);

        await WriteNode(session);
        await Assert.That(observer.Count).IsNotZero();
        await Assert.That(observer.Count).IsEqualTo(1);
    }

    private async Task WriteNode(ISession session)
    {
        var reader = fixture.GetService<IEntityBrowser<Gantry>>()!;
        IEntityNode node = await reader.BrowseEntityNode(fixture.Token);
        var childToWrite = node.PropertyStates.First(e => e.BrowseName.Name.Equals(nameof(Gantry.IntValue)));

        await session.WriteAsync(null, new WriteValueCollection
        {
            new WriteValue
            {
                NodeId = childToWrite.NodeId,
                AttributeId = Attributes.Value,
                Value = new DataValue
                {
                    Value = 2
                }
            }
        }, fixture.Token);
    }
}