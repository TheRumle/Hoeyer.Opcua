using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Extensions;

public static class LoggingExtensions
{
    /// <summary>
    /// Creates a scope with message <paramref cref="scope"/> and <paramref cref="scopeArgs"/>, then executes the <paramref name="action"/>. If an exception occurs it is logged and <b>rethrows the exception</b> 
    /// </summary>
    /// <param name="logger">the logger to use when creating the scope</param>
    /// <param name="scope">the scope name, taking a format string.</param>
    /// <param name="scopeArgs">the arguments for the formatted <paramref name="scope"/></param>
    /// <param name="action"></param>
    /// <param name="logLevel">The log-level to use</param>
    ///  <example><code>mylogger.LogWithScope("Unstable method with args {@Arg1}, @{Arg2}", arg1, arg2, () => {unstableMethod()}) </code></example>
    [SuppressMessage("Maintainability", "S2139", Justification = "The stack trace should not be contamined with logging information.")]
    public static T LogWithScope<T>(this ILogger logger, LogLevel logLevel, Func<T> action, string scope, params object[] scopeArgs)
    {
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        using (logger.BeginScope(scope, scopeArgs))
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                logger.Log(logLevel, ex, "An exception was thrown: \n");
                throw;
            }
        }
    }
    
    [SuppressMessage("Maintainability", "S2139", Justification = "The stack trace should not be contamined with logging information.")]
    public static void LogWithScope(this ILogger logger, LogLevel logLevel, Action action, string scope, params object[] scopeArgs)
    {
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        using (logger.BeginScope(scope, scopeArgs))
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                logger.Log(logLevel, ex, "An exception was thrown: \n");
                throw;
            }
        }
    }
    
    /// <summary>
    /// Creates a scope with message <paramref cref="scope"/> and <paramref cref="scopeArgs"/>, then executes the <paramref name="action"/>. If an exception occurs it is logged and <b>rethrows the exception</b> 
    /// </summary>
    /// <param name="logger">the logger to use when creating the scope</param>
    /// <param name="scope">the scope name, taking a format string.</param>
    /// <param name="scopeArgs">the arguments for the formatted <paramref name="scope"/></param>
    /// <param name="action"></param>
    ///  <example><code>mylogger.LogWithScope("Unstable method with args {@Arg1}, @{Arg2}", arg1, arg2, () => {unstableMethod()}) </code></example>
    public static void LogErrorWithScope(this ILogger logger, Action action, string scope, params object[] scopeArgs) 
        => logger.LogWithScope(LogLevel.Error, action, scope, scopeArgs);

    /// <summary>
    /// Creates a scope with message <paramref cref="scope"/> and <paramref cref="scopeArgs"/>, then executes the <paramref name="func"/>. If an exception occurs it is logged and <b>rethrows the exception</b>.
    /// </summary>
    /// <param name="logger">the logger to use when creating the scope</param>
    /// <param name="scope">the scope name, taking a format string.</param>
    /// <param name="scopeArgs">the arguments for the formatted <paramref name="scope"/></param>
    /// <param name="func"></param>
    /// <returns>Whatever <paramref name="func"/> returns</returns>
    ///  <example><code>mylogger.LogWithScope("Unstable method with args {@Arg1}, @{Arg2}", arg1, arg2, () => {unstableMethod()}) </code></example>
    public static T LogErrorWithScope<T>(this ILogger logger, Func<T> func, string scope, params object[] scopeArgs) 
        => logger.LogWithScope(LogLevel.Error, func, scope, scopeArgs);

}