using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

internal sealed class CopySubscriptionTransferStrategy : ISubscriptionTransferStrategy
{
    public async Task TransferSubscriptionsBetween(IAgentSession oldSession, IAgentSession newSession)
    {
        List<Task> tasks = new();
        foreach (var oldSub in oldSession.AgentSubscriptions.ToList())
        {
            var newSub = CloneSubscription(oldSession, oldSub);
            newSession.Session.AddSubscription(newSub);
            tasks.Add(newSub.CreateAsync());
        }

        await Task.WhenAll(tasks.ToArray());
    }

    private static AgentSubscription CloneSubscription(IAgentSession oldSession, AgentSubscription oldSub)
    {
        var newSub = new AgentSubscription(oldSession, oldSub.Callback)
        {
            PublishingInterval = oldSub.PublishingInterval,
            KeepAliveCount = oldSub.KeepAliveCount,
            LifetimeCount = oldSub.LifetimeCount,
            MaxNotificationsPerPublish = oldSub.MaxNotificationsPerPublish,
            Priority = oldSub.Priority,
            DisplayName = oldSub.DisplayName
        };

        foreach (var oldItem in oldSub.AgentItems)
        {
            var newItem = new MonitoredAgentItem(oldItem)
            {
                StartNodeId = oldItem.StartNodeId,
                AttributeId = oldItem.AttributeId,
                DisplayName = oldItem.DisplayName,
                MonitoringMode = oldItem.MonitoringMode,
                SamplingInterval = oldItem.SamplingInterval,
                QueueSize = oldItem.QueueSize,
                DiscardOldest = oldItem.DiscardOldest
            };

            foreach (var subscriber in oldItem.Subscribers)
            {
                newItem.Notification += subscriber;
            }

            foreach (var subscriber in oldItem.Subscribers)
            {
                oldItem.Notification -= subscriber;
            }
        }

        return newSub;
    }
}