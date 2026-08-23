using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging.Subscriptions.ChannelBased;

public sealed record ChannelBasedSubscription<T> : IMessageSubscription<T>
{
    public readonly string ConsumerName;
    public readonly Guid Id;
    private readonly Channel<IMessage<T>> _channel;
    private readonly IMessageConsumer<T> _consumer;
    private readonly CancellationTokenSource _cts = new();
    private readonly ILogger _logger;
    private readonly IDisposable? _loggingScope;
    private readonly Action? _onDispose;
    private readonly Task _processingTask;

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
        _processingTask = ProcessQueueAsync(_cts.Token);
        ConsumerName = _consumer.GetType().Name;

        _processingTask.ContinueWith(
            t => _logger.LogError(t.Exception, "Subscription processing failed."),
            TaskContinuationOptions.OnlyOnFaulted);

        _loggingScope = _logger.BeginScope(new[]
        {
            new KeyValuePair<string, object?>(nameof(SubscriptionId), SubscriptionId),
            new KeyValuePair<string, object?>(nameof(Id), Id),
            new KeyValuePair<string, object?>(nameof(ConsumerName), ConsumerName)
        });
    }

    public Guid SubscriptionId { get; } = Guid.NewGuid();
    public bool IsCancelled { get; private set; }
    public bool IsPaused { get; private set; }

    public void Unpause() => IsPaused = false;
    public void Pause() => IsPaused = true;

    public void Forward(IMessage<T> message)
    {
        if (IsCancelled || IsPaused)
        {
            return;
        }

        _channel.Writer.TryWrite(message);
    }

    public void Dispose()
    {
        if (IsCancelled)
        {
            return;
        }

        IsCancelled = true;
        _cts.Cancel();
        _channel.Writer.TryComplete();
        _logger.LogDebug("Subscription cancelled");
        _onDispose?.Invoke();
        _loggingScope?.Dispose();
    }


    private async Task ProcessQueueAsync(CancellationToken token)
    {
        try
        {
            await foreach (var message in _channel.Reader.ReadAllAsync(token))
            {
                if (IsCancelled || IsPaused)
                {
                    continue;
                }

                _consumer.Consume(message);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Subscription cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from channel");
        }
    }
}