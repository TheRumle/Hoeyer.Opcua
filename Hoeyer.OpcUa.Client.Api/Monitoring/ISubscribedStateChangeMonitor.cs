using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface ISubscribedStateChangeMonitor<T>
{
    IMessageSubscription Subscription { get; }
    ChannelReader<IMessage<T>> StateChangeChannel { get; }
}