using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Types;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

public static class LoggingExtensions
{
    [Pure]
    public static ILogLevelSelected LogCaughtExceptionAs(this ILogger logger, LogLevel level,
        Func<Exception, Exception>? customExceptionMapper = null)
    {
        return new LoggingSetup(logger, level, customExceptionMapper ?? Functionals.Identity);
    }


    public static AggregateException? TryForEach<T>(this ILogger logger, IEnumerable<T> elems,
        Action<T> action, Action<T, Exception>? onError = null, Action? onSuccess = null,
        Action<T>? onEachSuccess = null)
    {
        List<Exception> exceptions = new List<Exception>();
        foreach (var e in elems)
        {
            var err = logger.Try(() => action.Invoke(e), onError == null ? null : ex => onError.Invoke(e, ex));
            if (err is not null)
            {
                exceptions.Add(err);
            }
            else
            {
                onEachSuccess?.Invoke(e);
            }
        }

        if (exceptions.Count == 0)
        {
            onSuccess?.Invoke();
        }

        return exceptions.Count > 0 ? new AggregateException(exceptions) : null;
    }


    public static Exception? Try(this ILogger logger, Action action, Action<Exception>? errHandle = null,
        Action? onSuccess = null)
    {
        try
        {
            action.Invoke();
            onSuccess?.Invoke();
            return null;
        }
        catch (Exception e)
        {
            if (errHandle != null)
            {
                errHandle.Invoke(e);
            }
            else
            {
                logger.LogError(e, null);
            }

            return e;
        }
    }

    public static T? Try<T>(this ILogger logger, Func<T> action)
    {
        try
        {
            return action.Invoke();
        }
        catch (Exception e)
        {
            logger.LogError(e, null);
            return default;
        }
    }

    public static Task<T> TryAsync<T>(this ILogger logger, Func<Task<T>> action)
    {
        Task<T> LogError(Task<T> t)
        {
            logger.LogError(t.Exception!, null);
            return t;
        }

        Task<T> LogCancelled(Task<T> t)
        {
            logger.LogInformation("Cancelled");
            return t;
        }

        return action.Invoke().ContinueWith(t => t.Status switch
        {
            TaskStatus.Faulted => LogError(t),
            TaskStatus.Canceled => LogCancelled(t),
            TaskStatus.RanToCompletion => t,
            _ => throw new InvalidOperationException("Unhandled task status")
        }).Unwrap();
    }

    public static T? LogWithScope<T>(this ILogger logger, object scope, Func<T> t)
    {
        logger.BeginScope(scope);
        return logger.Try(t);
    }

    public static Task<T> LogWithScopeAsync<T>(this ILogger logger, object scope, Func<Task<T>> t)
    {
        logger.BeginScope(scope);
        return logger.TryAsync(t);
    }

    public static Exception TryOrGetError(this ILogger logger, Action action, Action? onSuccess = null)
    {
        try
        {
            action.Invoke();
            onSuccess?.Invoke();
            return null;
        }
        catch (Exception e)
        {
            logger.LogError(e, null);
            return e;
        }
    }
}