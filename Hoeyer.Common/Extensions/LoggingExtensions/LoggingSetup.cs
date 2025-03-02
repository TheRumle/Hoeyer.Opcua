using System;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

internal class LoggingSetup(ILogger logger, LogLevel logLevel) : ILogLevelSelected, IMessageSelected, IScopeAndMessageSelected, IScopeSelected
{
    private string? _scope;
    private object[]? _scopeArgs;
    
    private string? _message;
    private object[]? _messageArgs;
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
    public T WhenExecuting<T>(Func<T> action)
    {
        if (HasScope)
        {
            return ExecuteAndLogWithScope(action);
        }

        return ExecuteAndLog(action);
    }

    private void ExecuteAndLogWithScope(Action action)
    {
        using IDisposable a = logger.BeginScope(_scope);
        ExecuteAndLog(action);
    }
    
    private T ExecuteAndLogWithScope<T>(Func<T> action)
    {
        using IDisposable a = logger.BeginScope(_scope, _scopeArgs);
        return ExecuteAndLog(action);
    }
    
    private T ExecuteAndLog<T>(Func<T> action)
    {
        try
        {
            return action.Invoke();
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



    /// <inheritdoc />
    IScopeAndMessageSelected IScopeSelected.WithErrorMessage(string message, params object[] messageArguments)
    {
        this._message = message;
        this._messageArgs = messageArguments;
        return this;
    }

    /// <inheritdoc />
    public IScopeSelected WithScope(string scopeTitle, params object[] scopeArguments)
    {
        this._scope = scopeTitle;
        this._scopeArgs = scopeArguments;
        return this;
    }

    /// <inheritdoc />
    IScopeAndMessageSelected IMessageSelected.WithScope(string scopeTitle, params object[] scopeArguments)
    {
        this._scope = scopeTitle;
        this._scopeArgs = scopeArguments;
        return this;
    }

    /// <inheritdoc />
    IMessageSelected ILogLevelSelected.WithErrorMessage(string message, params object[] messageArguments)
    {
        this._message = message;
        this._messageArgs = messageArguments;
        return this;
    }


    private void LogException(Exception e)
    {
        logger.Log(logLevel, e, _message, _messageArgs);
    }
}