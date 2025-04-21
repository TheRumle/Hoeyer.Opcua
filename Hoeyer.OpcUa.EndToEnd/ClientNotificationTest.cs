using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Monitoring;
using Hoeyer.OpcUa.Core.Extensions;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.TestApplication;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.EndToEndTest;

[TestSubject(typeof(EntitySubscription<>))]
[ClassDataSource<ApplicationFixture>]
public sealed class ClientNotificationTest(ApplicationFixture fixture)
{
    [Test]
    public async Task WhenWritingNode_ObserverIsNotified()
    {
        var observer = new TestSubscriber<Gantry>();
        var monitor = await fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        _ = await monitor.SubscribeToChange(observer);
        
        await WriteNode();
        await Assert.That(observer.Count).IsNotZero();
        await Assert.That(observer.Count).IsEqualTo(1);
    }
    
    [Test]
    public async Task WhenWritingNodeTwice_ObserverIsNotified_Twice()
    {
        var observer = new TestSubscriber<Gantry>();
        var monitor = await fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        _ = await monitor.SubscribeToChange(observer);
        
        await WriteNode();
        await WriteNode();
        await Assert.That(observer.Count).IsEqualTo(2);
    }

    
    [Test]
    public async Task WhenPausingNotification_ObserverIsNotNotified()
    {
        var observer = new TestSubscriber<Gantry>();
        var monitor = await fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        var subscription = await monitor.SubscribeToChange(observer);
        subscription.Pause();
        await WriteNode();
        await Assert.That(observer.Count).IsZero();
    }
    
    [Test]
    public async Task WhenPausingUnpausingNotification_ObserverIsNotifiedOnce()
    {
        var observer = new TestSubscriber<Gantry>();
        var monitor = await fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        var subscription = await monitor.SubscribeToChange(observer);
        subscription.Pause();
        await WriteNode();
        subscription.Unpause();
        await WriteNode();
        await Assert.That(observer.Count).IsEqualTo(1);
    }
    
        
    [Test]
    public async Task WhenSubscriptionIsCancelled_ObserverIsNotNotified()
    {
        var observer = new TestSubscriber<Gantry>();
        var monitor = await fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        var subscription = await monitor.SubscribeToChange(observer);
        subscription.Dispose();
        await WriteNode();
        await Assert.That(observer.Count).IsZero();
    }

    private async Task WriteNode()
    {
        var session = await fixture.CreateSession(Guid.NewGuid().ToString());
        var reader = await fixture.GetService<IEntityBrowser<Gantry>>()!;
        var node = await reader.BrowseEntityNode(session, fixture.Token);
        var childToWrite = node.Children.First(e => e.BrowseName.Name.Equals(nameof(Gantry.IntValue)));

        await session.WriteAsync(null, new WriteValueCollection
        {
            new WriteValue
            {
                NodeId = childToWrite.NodeId.AsNodeId(session.NamespaceUris),
                AttributeId = Attributes.Value,
                Value = new DataValue
                {
                    Value = 2
                }
            }
        }, fixture.Token);
    }
}