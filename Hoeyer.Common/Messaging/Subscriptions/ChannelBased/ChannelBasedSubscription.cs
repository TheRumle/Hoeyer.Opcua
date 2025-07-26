using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging.Api;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging.Subscriptions.ChannelBased;

public sealed record ChannelBasedSubscription<T> : IMessageSubscription<T>
{
    private readonly Channel<IMessage<T>> _channel;
    private readonly IMessageConsumer<T> _consumer;
    private readonly CancellationTokenSource _cts = new();
    private readonly ILogger _logger;
    private readonly Action? _onDispose;
    public readonly Guid Id;

    public ChannelBasedSubscription(Guid id,
        IMessageConsumer<T> consumer,
        Channel<IMessage<T>> channel,
        ILogger logger,
        Action<ChannelBasedSubscription<T>>? disposeCallback)
    {
        _onDispose = () => disposeCallback?.Invoke(this);
        Id = id;
        _consumer = consumer;
        _channel = channel;
        _logger = logger;
        Task.Run(() => ProcessQueueAsync(_cts.Token), _cts.Token);
    }

    public Guid SubscriptionId { get; } = Guid.NewGuid();
    public bool IsCancelled { get; private set; }
    public bool IsPaused { get; private set; }

    public void Unpause() => IsPaused = false;
    public void Pause() => IsPaused = true;

    public void Forward(IMessage<T> message)
    {
        if (IsCancelled || IsPaused) return;
        _channel.Writer.TryWrite(message);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        IsCancelled = true;
        _channel.Writer.TryComplete();
        _logger.LogDebug("Subscription cancelled with owner '{@Name}'", _consumer.GetType().Name);
        _cts.Dispose();
        _onDispose?.Invoke();
    }


    private async Task ProcessQueueAsync(CancellationToken token)
    {
        try
        {
            await foreach (IMessage<T>? message in _channel.Reader.ReadAllAsync(token))
            {
                if (IsCancelled || IsPaused) continue;
                _consumer.Consume(message);
            }
        }
        catch (OperationCanceledException e)
        {
            // Expected during shutdown of manager
            _logger.LogDebug(e, "{@Subscription} has been cancelled", SubscriptionId.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured when processing message from channel.");
        }
    }
}