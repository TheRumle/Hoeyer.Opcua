using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.Common.Messaging.Subscriptions.ChannelBased;
using JetBrains.Annotations;

namespace Hoeyer.Common.Test.Messaging;

[TestSubject(typeof(SubscriptionManager<>))]
[TestSubject(typeof(ISubscriptionManager<>))]
[TestSubject(typeof(IMessageSubscription<>))]
public abstract class SubscriptionSystemTest(IMessageSubscriptionFactory<int> factory)
{
    private const int TimeoutMillis = 5000;
    private readonly Random _rand = new(46378919);

    private readonly SubscriptionManager<int> publisher = new(factory);

    public static IEnumerable<Func<(int consumers, int messages)>> IncreasingLoad()
    {
        for (int i = 1; i < 13; i++)
        {
            for (int j = 1; j < i; j++)
            {
                yield return () => ((int)Math.Pow(i, 2), (int)Math.Pow(j, 2));
            }
        }
    }

    [Test]
    public async Task WhenSubscriptionPaused_DoesNotCallSubscriber()
    {
        var _subscriber = new TestSubscriber(0, CancellationToken.None);
        var messageSubscription = (ChannelBasedSubscription<int>)publisher.Subscribe(_subscriber);
        messageSubscription.Pause();
        publisher.Publish(_rand.Next());
        await Assert.That(_subscriber.Count).IsEqualTo(0);
    }

    [Test]
    [Timeout(TimeoutMillis)]
    public async Task WhenSubscriberActive_SubscriberIsCalled(CancellationToken token)
    {
        var subscriber = new TestSubscriber(1, token);
        _ = publisher.Subscribe(subscriber);
        publisher.Publish(_rand.Next());
        await subscriber.HasWantedCalls;
    }

    [Test]
    [Timeout(TimeoutMillis)]
    public async Task WhenUnsubscribing_NumberOfSubscriptions_Decreases(CancellationToken token)
    {
        var subscriber = new TestSubscriber(1, token);
        IMessageSubscription<int> subscription = publisher.Subscribe(subscriber);
        var before = publisher.Collection.ActiveSubscriptionsCount;
        subscription.Dispose();
        await Assert.That(publisher.Collection.ActiveSubscriptionsCount).IsLessThan(before);
    }

    [Test]
    public async Task WhenUnsubscribing_DoesNotCallSubscriber()
    {
        var _subscriber = new TestSubscriber(1, CancellationToken.None);
        var subscription = publisher.Subscribe(_subscriber);
        subscription.Dispose();
        publisher.Publish(_rand.Next());
        await Assert.That(_subscriber.Count).IsEqualTo(0);
    }

    [Test]
    [MethodDataSource(nameof(IncreasingLoad))]
    public void CanHandleManyRequests_With_Changing_Subscribers((int consumers, int requests) state)
    {
        var (consumers, requests) = state;
        List<TestSubscriber> subscribers = new();
        for (int i = 0; i < consumers; i++)
        {
            var s = new TestSubscriber(10, CancellationToken.None);
            subscribers.Add(s);
            var sub = publisher.Subscribe(s);
            s.MessageSubscription = sub;
        }

        for (int i = 0; i < requests; i++)
        {
            var value = _rand.Next(0, consumers);
            if (subscribers[value].MessageSubscription.IsPaused)
            {
                subscribers[value].MessageSubscription.Pause();
            }
            else
            {
                subscribers[value].MessageSubscription.Unpause();
            }

            publisher.Publish(value);
        }
    }


    [Test]
    [Timeout(TimeoutMillis * 2)]
    public async Task CanHandleConcurrent_AddingAndRemoving(CancellationToken _)
    {
        var cts = new CancellationTokenSource();
        (Thread addThread, Thread publish) = CreateSimulationThreads(cts.Token);
        cts.CancelAfter(2000);
        addThread.Start();
        publish.Start();
        await cts.Token.WaitForCancellationAsync();
        addThread.Join();
        publish.Join();
        cts.Dispose();
    }

    private (Thread add, Thread publish) CreateSimulationThreads(CancellationToken token)
    {
        var subscriptions = new ConcurrentBag<IMessageSubscription>();
        Action<IMessageSubscription> pauseUnpause = sub =>
        {
            var action = _rand.Next(3);
            switch (action)
            {
                case 1:
                    sub.Pause();
                    break;
                case 2:
                    sub.Unpause();
                    break;
                default:
                    sub.Dispose();
                    break;
            }
        };

        var addThread = new Thread(() =>
        {
            while (!token.IsCancellationRequested)
            {
                IMessageSubscription<int> sub = publisher.Subscribe(new TestSubscriber(1, token));
                subscriptions.Add(sub);
                Thread.Sleep(TimeSpan.FromMilliseconds(60));
                pauseUnpause.Invoke(subscriptions.Skip(_rand.Next(0, subscriptions.Count)).First());
            }
        });

        var publishThread = new Thread(() =>
        {
            while (!token.IsCancellationRequested)
            {
                publisher.Publish(_rand.Next());
            }
        });
        return (addThread, publishThread);
    }

    private sealed class TestSubscriber(int wantedCalls, CancellationToken token) : IMessageConsumer<int>
    {
        private readonly TaskCompletionSource<bool> _tcs = new(token);
        public int Count { get; private set; }
        public IMessageSubscription MessageSubscription { get; set; }
        public Task HasWantedCalls => _tcs.Task;

        public void Consume(IMessage<int> message)
        {
            Count += 1;
            if (Count >= wantedCalls) _tcs.TrySetResult(true);
        }
    }
}