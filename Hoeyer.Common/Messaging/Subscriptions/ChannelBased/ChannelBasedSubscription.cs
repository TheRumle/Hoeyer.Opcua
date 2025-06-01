using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.Common.Messaging.Api;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging.Subscriptions.ChannelBased;

public sealed record ChannelBasedSubscription<T> : IMessageSubscription<T>
{
    private readonly Channel<IMessage<T>> _channel;
    private readonly IMessageConsumer<T> _consumer;
    private readonly IMessageUnsubscribable _creator;
    private readonly CancellationTokenSource _cts = new();
    private readonly ILogger _logger;
    public readonly Guid Id;

    public ChannelBasedSubscription(Guid id,
        IMessageUnsubscribable creator,
        IMessageConsumer<T> consumer,
        Channel<IMessage<T>> channel,
        ILogger logger)
    {
        Id = id;
        _consumer = consumer;
        _creator = creator;
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
        _logger.LogInformation("Subscription cancelled");
        _creator.Unsubscribe(this);
        IsCancelled = true;
        _cts.Dispose();
    }


    private async Task ProcessQueueAsync(CancellationToken token)
    {
        try
        {
            await foreach (IMessage<T>? message in _channel.Reader.ReadAllAsync(token))
            {
                if (IsCancelled || IsPaused) continue;
                _logger
                    .LogCaughtExceptionAs(LogLevel.Error)
                    .WithErrorMessage("Consumer threw error")
                    .WhenExecuting(() => _consumer.Consume(message));
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured when reading from channel");
        }
    }
}