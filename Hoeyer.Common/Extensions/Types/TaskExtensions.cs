using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.Common.Extensions.Types;

public static class TaskExtensions
{
    private const string UNEXPECTED_TASK_STATUS = "Unexpected task status.";



    public static Task<TOut> ThenAsync<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> mapper)
    {
        return task.ContinueWith(t => t.Status switch
            {
                TaskStatus.Faulted => Task.FromException<TOut>(t.Exception!),
                TaskStatus.Canceled => Task.FromCanceled<TOut>(new CancellationToken(true)),
                TaskStatus.RanToCompletion => Task.FromResult(mapper.Invoke(t.Result)),
                _ => throw new InvalidOperationException(UNEXPECTED_TASK_STATUS)
            })
            .Unwrap();
    }

    public static Task<TOut> ThenAsync<TIn, TOut>(this Task<TIn> task, Func<TIn, Task<TOut>> mapper)
    {
        return task.ContinueWith(t => t.Status switch
            {
                TaskStatus.Faulted => Task.FromException<TOut>(t.Exception!),
                TaskStatus.Canceled => Task.FromCanceled<TOut>(new CancellationToken(true)),
                TaskStatus.RanToCompletion => mapper.Invoke(t.Result),
                _ => throw new InvalidOperationException(UNEXPECTED_TASK_STATUS)
            })
            .Unwrap();
    }


    public static Task<TIn> ThenAsync<TIn>(this Task<TIn> task, Action<TIn> onSuccess)
    {
        var constActionThenReturn = (Task<TIn> r) =>
        {
            onSuccess(r.Result);
            return r;
        };

        return task.ContinueWith(t =>
                t.Status switch
                {
                    TaskStatus.Faulted => t,
                    TaskStatus.Canceled => t,
                    TaskStatus.RanToCompletion => constActionThenReturn.Invoke(t),
                    _ => throw new InvalidOperationException(UNEXPECTED_TASK_STATUS)
                })
            .Unwrap();
    }

}