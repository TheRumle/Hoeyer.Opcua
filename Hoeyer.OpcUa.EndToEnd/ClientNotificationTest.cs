using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Client.Application.Monitoring;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.TestEntities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

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
        var writer = fixture.ServiceProvider.GetService<IEntityWriter<Gantry>>()!;
        await writer.AssignEntityValues(new Gantry
        {
            AList = ["New Values", "Are good"],
            IntValue = 231,
            StringValue = "Helo there"
        });
        Thread.Sleep(4);
    }
}