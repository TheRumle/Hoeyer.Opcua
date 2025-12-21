using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;
using JetBrains.Annotations;
using Playground.Modelling.Models;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

[TestSubject(typeof(EntitySubscriptionManager<>))]
[TestSubject(typeof(CurrentEntityStateChannel<>))]
[ClassDataSource<SimulationFixture>]
public sealed class EntitySubscriptionManagerTest(SimulationFixture fixture)
{
    private const string EXPECTED_STRING_VALUE = "Hetsratsratsralo there";
    private static readonly int NumberOfGantryPropsChanged = 1;

    [Test]
    public async Task WhenWritingNode_ObserverIsNotified(CancellationToken token)
    {
        ((ICurrentEntityStateChannel<Gantry> channel, IMessageSubscription _),
            (CountingConsumer<Gantry> observer, IMessageSubscription _)) = await CreateSubscriberPair(token);
        await WriteNode();
        await channel.Reader.WaitToReadAsync(token);
        Gantry result = (await channel.Reader.ReadAsync(token)).Payload;

        await Assert.That(observer.Count).IsEqualTo(NumberOfGantryPropsChanged);
        await Assert.That(result.StringValue).IsEqualTo(EXPECTED_STRING_VALUE);
    }


    [Test]
    public async Task WhenPausingNotification_ObserverIsNotNotified(CancellationToken token)
    {
        ((ICurrentEntityStateChannel<Gantry> channel, IMessageSubscription _),
            (CountingConsumer<Gantry> observer, IMessageSubscription subscription)) = await CreateSubscriberPair(token);
        subscription.Pause();

        await WriteNode();
        await channel.Reader.WaitToReadAsync(token); //consume the published message
        await Assert.That(observer.Count).IsEqualTo(0);
    }


    [Test]
    public async Task WhenSubscriptionIsCancelled_ObserverIsNotNotified(CancellationToken token)
    {
        ((ICurrentEntityStateChannel<Gantry> channel, IMessageSubscription _),
            (CountingConsumer<Gantry> observer, IMessageSubscription subscription)) = await CreateSubscriberPair(token);
        subscription.Dispose();

        await WriteNode();
        await channel.Reader.WaitToReadAsync(token);
        await Assert.That(observer.Count).IsEqualTo(0);
    }

    private async Task<(ChannelSubscription channel, TestSubscriberSubscription testSubscriber)> CreateSubscriberPair(
        CancellationToken token)
    {
        var testSubscriber = new CountingConsumer<Gantry>();
        var monitor = fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        var channel = fixture.GetService<ICurrentEntityStateChannel<Gantry>>();
        return new ValueTuple<ChannelSubscription, TestSubscriberSubscription>(
            new ChannelSubscription(channel, await monitor.SubscribeToAllPropertyChanges(channel, token)),
            new TestSubscriberSubscription(testSubscriber,
                await monitor.SubscribeToAllPropertyChanges(testSubscriber, token)));
    }

    private async Task WriteNode()
    {
        var writer = fixture.GetService<IEntityWriter<Gantry>>();
        await writer.AssignEntityProperties([(nameof(Gantry.StringValue), EXPECTED_STRING_VALUE)]);
    }

    private sealed record ChannelSubscription(
        ICurrentEntityStateChannel<Gantry> Channel,
        IMessageSubscription Subscription);

    private sealed record TestSubscriberSubscription(
        CountingConsumer<Gantry> Consumer,
        IMessageSubscription Subscription);

    private sealed class CountingConsumer<T> : IMessageConsumer<T>
    {
        public int Count { get; private set; }
        public void Consume(IMessage<T> message) => Count += 1;
    }
}