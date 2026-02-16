using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface ISubscribedStateChangeMonitor
{
    IMessageSubscription Subscription { get; }
}

public interface ISubscribedStateChangeMonitor<T> : ISubscribedStateChangeMonitor
{
    ChannelReader<IMessage<T>> StateChangeChannel { get; }
}