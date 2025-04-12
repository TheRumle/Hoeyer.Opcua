using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;

namespace Hoeyer.Common.Extensions.Types;

public static class TaskExtensions
{
    private const string UNEXPECTED_TASK_STATUS = "Unexpected task status.";

    public static Task<Result<TOut>> AsResultTask<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> mapper)
    {
        return task.ContinueWith(t =>
                t.Status switch
                {
                    TaskStatus.Canceled => HandleCancelled<TOut>(),
                    TaskStatus.Faulted => HandleFailedTask<TIn, TOut>(t),
                    TaskStatus.RanToCompletion => HandleCompletedTask(t, mapper),
                    _ => throw new InvalidOperationException(UNEXPECTED_TASK_STATUS)
                })
            .Unwrap();
    }
    
    public static Task<Result<TIn>> AsResultTask<TIn>(this Task<TIn> task)
    {
        return task.ContinueWith(t =>
                t.Status switch
                {
                    TaskStatus.Canceled => HandleCancelled<TIn>(),
                    TaskStatus.Faulted => HandleFailedTask<TIn, TIn>(t),
                    TaskStatus.RanToCompletion => HandleCompletedTask(t, Functionals.Identity),
                    _ => throw new InvalidOperationException(UNEXPECTED_TASK_STATUS)
                })
            .Unwrap();
    }


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
    
    private static Task<Result<TOut>> HandleCompletedTask<TIn, TOut>(Task<TIn> result,  Func<TIn, TOut> mapper, Action<TIn>? onSuccess = null )
    {
        onSuccess?.Invoke(result.Result);
        return Task.FromResult(Result.Ok(mapper.Invoke(result.Result)));
    }
    
    private static Task<Result<TOut>> HandleFailedTask<TIn, TOut>(Task<TIn> result)
    {
        return Task.FromResult(Result.Fail<TOut>(result.Exception!.Message));
    }
    
    private static Task<Result<TOut>> HandleCancelled<TOut>() => Task.FromCanceled<Result<TOut>>(new CancellationToken(true));
}