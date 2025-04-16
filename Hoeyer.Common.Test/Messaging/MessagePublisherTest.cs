using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hoeyer.Common.Test.Messaging;

[TestSubject(typeof(MessagePublisher<>))]
public class MessagePublisherTest
{
    private readonly MessagePublisher<int> publisher = new(NullLoggerFactory.Instance.CreateLogger("Loggger"));
    private readonly TestSubscriber _subscriber = new();
    private readonly Random _rand = new(46378919);
    private sealed class TestSubscriber : IMessageSubscriber<int>
    {
        public int Count = 0; 
        public ISubscription Subscription { get; set; }
        public void OnMessagePublished(IMessage<int> message) => Count += 1;
    }
    
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
        ISubscription subscription = publisher.Subscribe(_subscriber);
        subscription.Pause();
        publisher.Publish(_rand.Next());
        await Assert.That(_subscriber.Count).IsEqualTo(0);
    }
    
    [Test]
    public async Task WhenSubscriberActive_SubscriberIsCalled()
    {
        _ = publisher.Subscribe(_subscriber);
        publisher.Publish(_rand.Next());
        await Assert.That(_subscriber.Count).IsEqualTo(1);
    }
    
    [Test]
    public async Task WhenUnsubscribing_NumberOfSubscriptions_Decreases()
    {
        var subscription = publisher.Subscribe(_subscriber);
        var before = publisher.NumberOfSubscriptions;
        subscription.Dispose();
        await Assert.That(publisher.NumberOfSubscriptions).IsLessThan(before);
    }

    [Test]
    public async Task WhenUnsubscribing_DoesNotCallSubscriber()
    {
        var subscription = publisher.Subscribe(_subscriber);
        subscription.Dispose();
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
            var s = new TestSubscriber();
            subscribers.Add(s);
            var sub = publisher.Subscribe(s);
            s.Subscription = sub;
        }

        for (int i = 0; i < requests; i++)
        {
            var value = _rand.Next(0, consumers);
            if (subscribers[value].Subscription.IsPaused)
            {
                subscribers[value].Subscription.Pause();
            }
            else
            { 
                subscribers[value].Subscription.Unpause();
            }
            publisher.Publish(value);
        }
    }
    
   
    [Test]
    public void CanHandleConcurrent_AddingAndRemoving()
    {
        var cts = new CancellationTokenSource();
        var (addThread, publish) = CreateSimulationThreads(cts);

        addThread.Start();
        publish.Start();
        Thread.Sleep(5000);
        cts.Cancel();
        addThread.Join();
        publish.Join();
    }

    private (Thread add,Thread publish) CreateSimulationThreads(CancellationTokenSource cts)
    {
        var subscriptions = new ConcurrentBag<ISubscription>();

        Action<ISubscription> pauseUnpause = (sub) =>
        {
            if (subscriptions.Count % 2 == 0)
            {
                var action = _rand.Next(3);
                if (action == 1)
                    sub.Pause();
                else
                    sub.Dispose();
            }
        };
        
        var addThread = new Thread(() =>
        {
            while (!cts.IsCancellationRequested)
            {
                var sub = publisher.Subscribe(new TestSubscriber());
                subscriptions.Add(sub);
                Thread.Sleep(TimeSpan.FromMilliseconds(60));
                pauseUnpause.Invoke(subscriptions.Skip(_rand.Next(0,subscriptions.Count)).First() );
            }
        });

        var publishThread = new Thread(() =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                publisher.Publish(_rand.Next());
            }
        });
        return (addThread, publishThread);
    }
}