using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.TestEntities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest;

[TestSubject(typeof(AgentSubscriptionManager<>))]
[TestSubject(typeof(CurrentAgentStateChannel<>))]
[NotInParallel(nameof(AgentSubscriptionManagerTest))]
[ClassDataSource<ApplicationFixture>]
public sealed class AgentSubscriptionManagerTest(ApplicationFixture fixture)
{
    private const string EXPECTED_STRING_VALUE = "Hetsratsratsralo there";
    private static readonly int NumberOfGantryPropsChanged = 1;


    [Test]
    [Timeout(10_000)]
    public async Task WhenWritingNode_ObserverIsNotified(CancellationToken token)
    {
        ((ICurrentAgentStateChannel<Gantry> channel, IMessageSubscription _),
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
        ((ICurrentAgentStateChannel<Gantry> channel, IMessageSubscription _),
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
        ((ICurrentAgentStateChannel<Gantry> channel, IMessageSubscription _),
            (CountingConsumer<Gantry> observer, IMessageSubscription subscription)) = await CreateSubscriberPair(token);
        subscription.Dispose();

        await WriteNode();
        await channel.Reader.WaitToReadAsync(token);
        await Assert.That(observer.Count).IsZero();
    }

    private async Task<(ChannelSubscription channel, TestSubscriberSubscription testSubscriber)> CreateSubscriberPair(
        CancellationToken token)
    {
        var testSubscriber = new CountingConsumer<Gantry>();
        var monitor = fixture.GetService<IAgentSubscriptionManager<Gantry>>();
        var channel = fixture.GetService<ICurrentAgentStateChannel<Gantry>>();
        return new ValueTuple<ChannelSubscription, TestSubscriberSubscription>(
            new ChannelSubscription(channel, await monitor.SubscribeToAllPropertyChanges(channel, token)),
            new TestSubscriberSubscription(testSubscriber,
                await monitor.SubscribeToAllPropertyChanges(testSubscriber, token)));
    }

    private async Task WriteNode()
    {
        var writer = fixture.ServiceProvider.GetService<IAgentWriter<Gantry>>()!;
        await writer.AssignAgentProperties([(nameof(Gantry.StringValue), EXPECTED_STRING_VALUE)]);
    }

    public sealed record ChannelSubscription(
        ICurrentAgentStateChannel<Gantry> Channel,
        IMessageSubscription Subscription);

    public sealed record TestSubscriberSubscription(
        CountingConsumer<Gantry> Consumer,
        IMessageSubscription Subscription);
}