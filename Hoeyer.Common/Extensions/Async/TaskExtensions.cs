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
}