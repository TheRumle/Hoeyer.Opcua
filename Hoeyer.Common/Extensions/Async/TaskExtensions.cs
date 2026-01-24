using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.Common.Extensions.Async;

public static class TaskExtensions
{
    public static Task WaitForCancellationAsync(this CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource<object>();
        token.Register(() => tcs.TrySetResult(null));
        return tcs.Task;
    }

    public static async Task<T> WithCancellation<T>(
        this Task<T> task,
        CancellationToken cancellationToken)
    {
        var cancellationTask = Task.Delay(Timeout.Infinite, cancellationToken);

        var completedTask = await Task.WhenAny(task, cancellationTask)
            .ConfigureAwait(false);

        if (completedTask == cancellationTask)
        {
            throw new OperationCanceledException(cancellationToken);
        }

        return await task.ConfigureAwait(false);
    }

    public static async Task WithCancellation(
        this Task task,
        CancellationToken cancellationToken)
    {
        var cancellationTask = Task.Delay(Timeout.Infinite, cancellationToken);

        var completedTask = await Task.WhenAny(task, cancellationTask)
            .ConfigureAwait(false);

        if (completedTask == cancellationTask)
        {
            throw new OperationCanceledException(cancellationToken);
        }

        await task.ConfigureAwait(false);
    }
}