namespace Hoeyer.Common.Extensions.Types;

public static class TaskExtensions
{
    private const string UNEXPECTED_TASK_STATUS = "Unexpected task status.";

    public static Task<TOut> SelectAsync<TOut>(this Task task, Func<TOut> mapper)
    {
        return task.ContinueWith(t => t.Status switch
            {
                TaskStatus.Faulted => Task.FromException<TOut>(t.Exception!),
                TaskStatus.Canceled => Task.FromCanceled<TOut>(new CancellationToken(true)),
                TaskStatus.RanToCompletion => Task.FromResult(mapper.Invoke()),
                var _ => throw new InvalidOperationException(UNEXPECTED_TASK_STATUS)
            })
            .Unwrap();
    }

    public static async ValueTask<TOut> SelectAsync<TOut>(this ValueTask task, Func<TOut> mapper)
    {
        try
        {
            await task.ConfigureAwait(false);
            return mapper.Invoke();
        }
        catch (OperationCanceledException oce)
        {
            return await new ValueTask<TOut>(Task.FromCanceled<TOut>(oce.CancellationToken));
        }
        catch (Exception ex)
        {
            return await new ValueTask<TOut>(Task.FromException<TOut>(ex));
        }
    }

    extension<TIn>(Task<TIn> task)
    {
        public Task<TOut> SelectAsync<TOut>(Func<TIn, TOut> mapper)
        {
            return task.ContinueWith(t => t.Status switch
                {
                    TaskStatus.Faulted => Task.FromException<TOut>(t.Exception!),
                    TaskStatus.Canceled => Task.FromCanceled<TOut>(new CancellationToken(true)),
                    TaskStatus.RanToCompletion => Task.FromResult(mapper.Invoke(t.Result)),
                    var _ => throw new InvalidOperationException(UNEXPECTED_TASK_STATUS)
                })
                .Unwrap();
        }

        public Task<TOut> SelectAsync<TOut>(Func<TIn, Task<TOut>> mapper) =>
            task.SelectAsync(mapper, CancellationToken.None);

        public Task<TOut> SelectAsync<TOut>(Func<TIn, Task<TOut>> mapper,
            CancellationToken ct)
        {
            return task.ContinueWith(t => t.Status switch
                {
                    TaskStatus.Faulted => Task.FromException<TOut>(t.Exception!),
                    TaskStatus.Canceled => Task.FromCanceled<TOut>(new CancellationToken(true)),
                    TaskStatus.RanToCompletion => mapper.Invoke(t.Result),
                    var _ => throw new InvalidOperationException(UNEXPECTED_TASK_STATUS)
                }, ct)
                .Unwrap();
        }

        public Task<TIn> SelectAsync(Action<TIn> onSuccess)
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
                        var _ => throw new InvalidOperationException(UNEXPECTED_TASK_STATUS)
                    })
                .Unwrap();
        }
    }

    extension<TIn>(ValueTask<TIn> task)
    {
        public async ValueTask<TOut> SelectAsync<TOut>(Func<TIn, TOut> mapper)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                return mapper.Invoke(result);
            }
            catch (OperationCanceledException oce)
            {
                return await new ValueTask<TOut>(Task.FromCanceled<TOut>(oce.CancellationToken));
            }
            catch (Exception ex)
            {
                return await new ValueTask<TOut>(Task.FromException<TOut>(ex));
            }
        }

        public ValueTask<TOut> SelectAsync<TOut>(Func<TIn, ValueTask<TOut>> mapper) =>
            task.SelectAsync(mapper, CancellationToken.None);

        public async ValueTask<TOut> SelectAsync<TOut>(Func<TIn, ValueTask<TOut>> mapper, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var result = await task.ConfigureAwait(false);
                return await mapper.Invoke(result).ConfigureAwait(false);
            }
            catch (OperationCanceledException oce)
            {
                return await new ValueTask<TOut>(Task.FromCanceled<TOut>(oce.CancellationToken));
            }
            catch (Exception ex)
            {
                return await new ValueTask<TOut>(Task.FromException<TOut>(ex));
            }
        }

        public async ValueTask<TIn> SelectAsync(Action<TIn> onSuccess)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                onSuccess(result);
                return result;
            }
            catch (OperationCanceledException oce)
            {
                return await new ValueTask<TIn>(Task.FromCanceled<TIn>(oce.CancellationToken));
            }
            catch (Exception ex)
            {
                return await new ValueTask<TIn>(Task.FromException<TIn>(ex));
            }
        }
    }
}