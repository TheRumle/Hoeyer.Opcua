using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

[OpcUaEntityService(typeof(ICurrentEntityStateChannel<>))]
public sealed class CurrentEntityStateChannel<T> : ICurrentEntityStateChannel<T>
{
    private readonly Channel<IMessage<T>> _channel;

    private readonly BoundedChannelOptions _options = new(1)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        AllowSynchronousContinuations = false,
        SingleWriter = true
    };

    public CurrentEntityStateChannel()
    {
        _channel = Channel.CreateBounded<IMessage<T>>(_options);
    }

    public int Capacity => _options.Capacity;


    /// <inheritdoc />
    public void Consume(IMessage<T> message) => _channel.Writer.TryWrite(message);

    public ChannelReader<IMessage<T>> Reader => _channel.Reader;
}