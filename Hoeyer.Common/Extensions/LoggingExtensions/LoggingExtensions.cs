using System;
using System.Diagnostics.Contracts;
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
}