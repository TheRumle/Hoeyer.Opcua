using System.Collections.Immutable;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Client.Application.Monitoring;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.opcUa.TestEntities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest;

[TestSubject(typeof(EntitySubscriptionManager<>))]
[TestSubject(typeof(EntityChangeChannel<>))]
[ClassDataSource<ApplicationFixture>]
public sealed class ClientNotificationTest(ApplicationFixture fixture)
{
    private readonly Gantry WantedGantry = new()
    {
        AList = ["New Values", "Are good"],
        IntValue = 111111111,
        StringValue = "Helo there"
    };


    [Test]
    public async Task WhenWritingNode_ObserverIsNotified(CancellationToken token)
    {
        var monitor = fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        var channel = fixture.GetService<IEntityChangeChannel<Gantry>>();
        _ = await monitor.SubscribeToChange(channel, token);
        await WriteNode();
        await channel.Reader.WaitToReadAsync(token);
        Gantry result = (await channel.Reader.ReadAsync(token)).Payload;
        await Assert.That(result.IntValue).IsEqualTo(WantedGantry.IntValue);
        await Assert.That(result.StringValue).IsEqualTo(WantedGantry.StringValue);
        await Assert.That(result.AList.ToImmutableSortedSet())
            .IsEquivalentTo(WantedGantry.AList.ToImmutableSortedSet());
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
        await writer.AssignEntityValues(WantedGantry);
    }
}