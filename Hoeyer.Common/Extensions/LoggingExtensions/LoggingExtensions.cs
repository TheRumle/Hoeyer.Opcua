using System;
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

    public static Task<T> LogWithScopeAsync<T>(this ILogger logger, object scope, Func<Task<T>> t)
    {
        logger.BeginScope(scope);
        return logger.TryAsync(t);
    }
}