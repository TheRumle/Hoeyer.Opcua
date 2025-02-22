using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Extensions;

public static class LoggingExtensions
{
    public static void LogResultError(this ILogger logger, string baseMessage, Result r)
    {
        foreach (var err in r.Errors) logger.LogError(err.Message);
    }
    
    public static void LogErrors(this IEnumerable<Result> results, ILogger logger)
    {
        foreach (var err in results.SelectMany(e=>e.Errors)) logger.LogError(err.Message);
    }
    
    public static void LogErrors<T>(this IEnumerable<Result<T>> results, ILogger logger)
    {
        foreach (var err in results.SelectMany(e=>e.Errors)) logger.LogError(err.Message);
    }
}