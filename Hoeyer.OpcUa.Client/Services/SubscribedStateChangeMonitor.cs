using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Client.Services;

public sealed record SubscribedStateChangeMonitor<T>
{
    internal SubscribedStateChangeMonitor(ChannelReader<IMessage<T>> stateChannel, IMessageSubscription subscription)
    {
        Subscription = subscription;
        StateChangeChannel = stateChannel;
    }

    public IMessageSubscription Subscription { get; }
    public ChannelReader<IMessage<T>> StateChangeChannel { get; }

    public void Deconstruct(out IMessageSubscription messageSubscription, out ChannelReader<IMessage<T>> channel)
    {
        messageSubscription = Subscription;
        channel = StateChangeChannel;
    }
}