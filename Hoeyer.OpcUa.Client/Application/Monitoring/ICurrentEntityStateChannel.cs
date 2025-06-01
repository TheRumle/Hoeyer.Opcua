using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

public interface ICurrentEntityStateChannel<T> : IMessageConsumer<T>
{
    public ChannelReader<IMessage<T>> Reader { get; }
}