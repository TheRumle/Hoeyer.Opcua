using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Monitoring;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.TestEntities;
using Hoeyer.OpcUa.Server.Application;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.EndToEndTest;

[TestSubject(typeof(EntitySubscriptionManager<>))]
[ClassDataSource<ApplicationFixture>]
public sealed class ClientNotificationTest(ApplicationFixture fixture)
{
    [Test]
    public async Task WhenWritingNode_ObserverIsNotified()
    {
        var observer = new TestSubscriber<Gantry>();
        var monitor = fixture.GetService<IEntitySubscriptionManager<Gantry>>();

        _ = await monitor.SubscribeToChange(observer);

        await WriteNode();
        Thread.Sleep(TimeSpan.FromMilliseconds(100));
        await Assert.That(observer.Count).IsNotZero();
        await Assert.That(observer.Count).IsEqualTo(1);
    }
    
    [Test]
    public async Task WhenPausingNotification_ObserverIsNotNotified()
    {
        var observer = new TestSubscriber<Gantry>();
        var monitor = fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        var subscription = await monitor.SubscribeToChange(observer);
        subscription.Pause();
        await WriteNode();
        await Assert.That(observer.Count).IsZero();
    }

        
    [Test]
    public async Task WhenSubscriptionIsCancelled_ObserverIsNotNotified()
    {
        var observer = new TestSubscriber<Gantry>();
        var monitor = fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        var subscription = await monitor.SubscribeToChange(observer);
        subscription.Dispose();
        await WriteNode();
        await Assert.That(observer.Count).IsZero();
    }

    private async Task WriteNode()
    {
        var session = await fixture.CreateSession(Guid.NewGuid().ToString());
        var reader = fixture.GetService<IEntityBrowser<Gantry>>()!;
        _ = fixture.GetService<ManagedEntityNodeSingletonFactory<Gantry>>()!;
        var node = await reader.BrowseEntityNode(fixture.Token);
        var childToWrite = node.PropertyByBrowseName[nameof(Gantry.IntValue)];

        await session.WriteAsync(null, new WriteValueCollection
        {
            new WriteValue
            {
                NodeId = childToWrite.NodeId,
                AttributeId = Attributes.Value,
                Value = new DataValue
                {
                    Value = 73289743892
                }
            }
        }, fixture.Token);
        
        Thread.Sleep(4);
    }
}