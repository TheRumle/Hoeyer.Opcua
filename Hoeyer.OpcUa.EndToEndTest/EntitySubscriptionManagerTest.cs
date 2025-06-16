using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Client.Application.Monitoring;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.TestEntities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest;

[TestSubject(typeof(EntitySubscriptionManager<>))]
[TestSubject(typeof(CurrentEntityStateChannel<>))]
[ClassDataSource<ApplicationFixture>]
[NotInParallel(nameof(EntitySubscriptionManagerTest))]
public sealed class EntitySubscriptionManagerTest(ApplicationFixture fixture)
{
    private const string EXPECTED_STRING_VALUE = "Hetsratsratsralo there";
    private static readonly int NumberOfGantryPropsChanged = 1;


    [Test]
    [Timeout(10_000)]
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
    [Timeout(10_000)]
    public async Task WhenPausingNotification_ObserverIsNotNotified(CancellationToken token)
    {
        ((ICurrentEntityStateChannel<Gantry> channel, IMessageSubscription _),
            (CountingConsumer<Gantry> observer, IMessageSubscription subscription)) = await CreateSubscriberPair(token);
        subscription.Pause();

        await WriteNode();
        await channel.Reader.WaitToReadAsync(token);
        await Assert.That(observer.Count).IsZero();
    }


    [Test]
    [Timeout(10_000)]
    public async Task WhenSubscriptionIsCancelled_ObserverIsNotNotified(CancellationToken token)
    {
        ((ICurrentEntityStateChannel<Gantry> channel, IMessageSubscription _),
            (CountingConsumer<Gantry> observer, IMessageSubscription subscription)) = await CreateSubscriberPair(token);
        subscription.Dispose();

        await WriteNode();
        await channel.Reader.WaitToReadAsync(token);
        await Assert.That(observer.Count).IsZero();
    }

    [Test]
    [Timeout(10_000)]
    public async Task WhenSubscribed_AndStateIsSame_DoesNotNotify(CancellationToken token)
    {
        (ChannelSubscription awaitableObserver, TestSubscriberSubscription countingObserver) =
            await CreateSubscriberPair(token);
        ICurrentEntityStateChannel<Gantry> channel = awaitableObserver.Channel;
        CountingConsumer<Gantry> observer = countingObserver.Consumer;

        await WriteNode();
        await channel.Reader.ReadAsync(token);
        await Assert.That(observer.Count).IsEqualTo(NumberOfGantryPropsChanged);

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            using var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(1));
            await WriteNode();
            await channel.Reader.ReadAsync(source.Token);
        });
    }


    private async Task<(ChannelSubscription channel, TestSubscriberSubscription testSubscriber)> CreateSubscriberPair(
        CancellationToken token)
    {
        var testSubscriber = new CountingConsumer<Gantry>();
        var monitor = fixture.GetService<IEntitySubscriptionManager<Gantry>>();
        var channel = fixture.GetService<ICurrentEntityStateChannel<Gantry>>();
        return new ValueTuple<ChannelSubscription, TestSubscriberSubscription>(
            new ChannelSubscription(channel, await monitor.SubscribeToChange(channel, token)),
            new TestSubscriberSubscription(testSubscriber, await monitor.SubscribeToChange(testSubscriber, token)));
    }

    private async Task WriteNode()
    {
        var writer = fixture.ServiceProvider.GetService<IEntityWriter<Gantry>>()!;
        await writer.AssignEntityProperties([(nameof(Gantry.StringValue), EXPECTED_STRING_VALUE)]);
    }

    private sealed record ChannelSubscription(
        ICurrentEntityStateChannel<Gantry> Channel,
        IMessageSubscription Subscription);

    private sealed record TestSubscriberSubscription(
        CountingConsumer<Gantry> Consumer,
        IMessageSubscription Subscription);
}