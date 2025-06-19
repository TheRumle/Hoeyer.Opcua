using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Connection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

internal sealed class CopySubscriptionTransferStrategy : ISubscriptionTransferStrategy
{
    public async Task TransferSubscriptionsBetween(IEntitySession oldSession, IEntitySession newSession)
    {
        foreach (EntitySubscription? oldSub in oldSession.EntitySubscriptions.ToList())
        {
            // Clone subscription settings
            var newSub = new EntitySubscription(oldSession)
            {
                PublishingInterval = oldSub.PublishingInterval,
                KeepAliveCount = oldSub.KeepAliveCount,
                LifetimeCount = oldSub.LifetimeCount,
                MaxNotificationsPerPublish = oldSub.MaxNotificationsPerPublish,
                Priority = oldSub.Priority,
                DisplayName = oldSub.DisplayName
            };

            foreach (MonitoredEntityItem? oldItem in oldSub.EntityItems)
            {
                var newItem = new MonitoredEntityItem(oldItem)
                {
                    StartNodeId = oldItem.StartNodeId,
                    AttributeId = oldItem.AttributeId,
                    DisplayName = oldItem.DisplayName,
                    MonitoringMode = oldItem.MonitoringMode,
                    SamplingInterval = oldItem.SamplingInterval,
                    QueueSize = oldItem.QueueSize,
                    DiscardOldest = oldItem.DiscardOldest
                };

                foreach (MonitoredItemNotificationEventHandler? subscriber in oldItem.Subscribers)
                {
                    newItem.Notification += subscriber;
                }

                foreach (MonitoredItemNotificationEventHandler? subscriber in oldItem.Subscribers)
                {
                    oldItem.Notification -= subscriber;
                }
            }

            newSession.Session.AddSubscription(newSub);
            await newSub.CreateAsync();
        }
    }
}