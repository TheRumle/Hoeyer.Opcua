using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

public interface ICurrentAgentStateChannel<T> : IMessageConsumer<T>
{
    public ChannelReader<IMessage<T>> Reader { get; }
}