using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using Hoeyer.Common.Messaging.Api;
using JetBrains.Annotations;

namespace Hoeyer.Common.Test.Messaging;

[TestSubject(typeof(MessagePublisher<>))]
public class MessagePublisherTest
{
    private readonly MessagePublisher<int> publisher = new();
    private readonly TestSubscriber _subscriber = new();
    private readonly Random _rand = new(46378919);
    private sealed class TestSubscriber : IMessageConsumer<int>
    {
        public int Count; 
        public IMessageSubscription MessageSubscription { get; set; }
        public void Consume(IMessage<int> changedProperties) => Count += 1;
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
        IMessageSubscription messageSubscription = publisher.Subscribe(_subscriber);
        messageSubscription.Pause();
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
    public void CanHandleConcurrent_AddingAndRemoving()
    {
        var cts = new CancellationTokenSource();
        var (addThread, publish) = CreateSimulationThreads(cts);

        addThread.Start();
        publish.Start();
        Thread.Sleep(2000);
        cts.Cancel();
        addThread.Join();
        publish.Join();
    }

    private (Thread add,Thread publish) CreateSimulationThreads(CancellationTokenSource cts)
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