using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

public static class LoggingExtensions
{
    [Pure]
    public static ILogLevelSelected LogCaughtExceptionAs(this ILogger logger, LogLevel level)
    {
        return new LoggingSetup(logger, level);
    }
}