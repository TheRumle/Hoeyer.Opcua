using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

internal sealed class LoggingSetup(ILogger logger, LogLevel logLevel, Func<Exception, Exception> customExceptionMapper)
    : ILogLevelSelected, IMessageSelected, IScopeAndMessageSelected, IScopeSelected
{
    private string? _message;
    private object[]? _messageArgs;
    private string? _scope;
    private object[]? _scopeArgs;
    private bool HasScope => _scope != null;
    public ILogger Logger { get; } = logger;

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
    public IScopeSelected WithScope(object scopeArguments)
    {
        this._scopeArgs = [scopeArguments];
        this._message = "";
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

    private void Log<T>(LogLevel logResultAs, T a)
    {
        Logger.Log(logResultAs, "Got {Values}", a);
    }
    
    private void ExecuteAndLogWithScope(Action action)
    {
        if (_scope != null)
        {
            using var a = Logger.BeginScope(_scope);
            ExecuteAndLog(action);
        }
        else
        {
            ExecuteAndLog(action);
        }
    }

    private T ExecuteAndLogWithScope<T>(Func<T> action)
    {
        if (_scope != null)
        {
            using var a = Logger.BeginScope(_scope, _scopeArgs ?? []);
            return ExecuteAndLog(action);
        }

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
            throw customExceptionMapper.Invoke(ex);
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
            throw customExceptionMapper.Invoke(ex);
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
            throw customExceptionMapper.Invoke(ex);
        }
    }

    private void LogException(Exception e)
    {
        Logger.Log(logLevel, e, _message, _messageArgs ?? []);
    }
}