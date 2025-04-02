using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

internal sealed class LoggingSetup(ILogger logger, LogLevel logLevel)
    : ILogLevelSelected, IMessageSelected, IScopeAndMessageSelected, IScopeSelected
{
    private string? _message;
    private object[]? _messageArgs;
    private string? _scope;
    private object[]? _scopeArgs;
    private bool HasScope => _scope != null;

    public void WhenExecuting(Action action)
    {
        if (HasScope)
        {
            ExecuteAndLogWithScope(action);
            return;
        }

        ExecuteAndLog(action);
    }

    /// <inheritdoc />
    public T WhenExecuting<T>(Func<T> action, LogLevel logResultAs = LogLevel.None)
    {
        if (HasScope)
        {
            var a = ExecuteAndLogWithScope(action);
            if (EqualityComparer<T>.Default.Equals(a, default!))
            {
                Log(logResultAs, a);
            }

            return a;
        }

        var res = ExecuteAndLog(action);
        if (EqualityComparer<T>.Default.Equals(res, default!))
        {
            Log(logResultAs, res);
        }

        return res;
    }

    private void Log<T>(LogLevel logResultAs, T a)
    {
        logger.Log(logResultAs, "Got {Values}", a);
    }

    public async Task<T> WhenExecutingAsync<T>(Func<Task<T>> action, LogLevel logResultAs = LogLevel.None)
    {
        if (HasScope)
        {
            var a = await ExecuteAndLogWithScope(action);
            if (EqualityComparer<T>.Default.Equals(a, default!))
            {
                Log(logResultAs, a);
            }

            return a;
        }

        var res = await ExecuteAndLog(action);
        if (EqualityComparer<T>.Default.Equals(res, default!))
        {
            Log(logResultAs, res);
        }
        return res;
    }
    
    /// <inheritdoc />
    public IScopeSelected WithScope(string scopeTitle, params object[] scopeArguments)
    {
        _scope = scopeTitle;
        _scopeArgs = scopeArguments;
        return this;
    }

    /// <inheritdoc />
    IMessageSelected ILogLevelSelected.WithErrorMessage(string message, params object[] messageArguments)
    {
        _message = message;
        _messageArgs = messageArguments;
        return this;
    }

    /// <inheritdoc />
    IScopeAndMessageSelected IMessageSelected.WithScope(string scopeTitle, params object[] scopeArguments)
    {
        _scope = scopeTitle;
        _scopeArgs = scopeArguments;
        return this;
    }


    /// <inheritdoc />
    IScopeAndMessageSelected IScopeSelected.WithErrorMessage(string message, params object[] messageArguments)
    {
        _message = message;
        _messageArgs = messageArguments;
        return this;
    }

    private void ExecuteAndLogWithScope(Action action)
    {
        using var a = logger.BeginScope(_scope);
        ExecuteAndLog(action);
    }

    private T ExecuteAndLogWithScope<T>(Func<T> action)
    {
        using var a = logger.BeginScope(_scope, _scopeArgs);
        return ExecuteAndLog(action);
    }

    private T ExecuteAndLog<T>(Func<T> func)
    {
        try
        {
            return func.Invoke();
        }
        catch (Exception ex)
        {
            LogException(ex);
            throw;
        }
    }
    
    
    [SuppressMessage("SonarQube", "S5034", Justification = "If the operation fails, the error is caught and logged")]
    private T ExecuteAndLog<T>(Task<T> func)
    {
        try
        {
            return func.Result;
        }
        catch (Exception ex)
        {
            LogException(ex);
            throw;
        }
    }

    private void ExecuteAndLog(Action action)
    {
        try
        {
            action.Invoke();
        }
        catch (Exception ex)
        {
            LogException(ex);
            throw;
        }
    }

    private void LogException(Exception e)
    {
        logger.Log(logLevel, e, _message, _messageArgs);
    }
}