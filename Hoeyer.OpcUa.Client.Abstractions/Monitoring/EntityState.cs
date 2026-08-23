namespace Hoeyer.OpcUa.Client.Abstractions.Monitoring;

public class EntityState<T>
{
    private readonly CancellationTokenSource _cts = new();
    private readonly IStateChangeObserver<T> _observer;

    public EntityState(IStateChangeObserver<T> observer)
    {
        _observer = observer;
        _ = ListenAsync();
    }

    public T? Current { get; private set; }

    public event Action<T>? OnStateChanged;

    private async Task ListenAsync()
    {
        // first get the channel reader from your observer
        var reader = (await _observer.BeginObserveAsync()).StateChangeChannel;

        // now enumerate it asynchronously
        await foreach (var message in reader.ReadAllAsync(_cts.Token))
        {
            Current = message.Payload;
            OnStateChanged?.Invoke(Current);
        }
    }

    public void Dispose() => _cts.Cancel();
}