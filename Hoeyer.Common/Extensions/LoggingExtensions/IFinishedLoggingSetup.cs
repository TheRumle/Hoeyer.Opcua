using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

public interface IFinishedLoggingSetup
{
    void WhenExecuting(Action action);

    [Pure]
    T WhenExecuting<T>(Func<T> action, LogLevel logResultAs = LogLevel.None);

    [Pure]
    public Task<T> WhenExecutingAsync<T>(Func<Task<T>> action, LogLevel logResultAs = LogLevel.None);

}