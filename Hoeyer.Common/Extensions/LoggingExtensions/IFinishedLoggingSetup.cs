using System;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

public interface IFinishedLoggingSetup
{
    void WhenExecuting(Action action);

    [Pure]
    T WhenExecuting<T>(Func<T> action, LogLevel logResultAs = LogLevel.None);
}