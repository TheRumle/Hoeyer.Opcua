using System;
using System.Diagnostics.Contracts;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

public interface IFinishedLoggingSetup
{
    void WhenExecuting(Action action);

    [Pure]
    T WhenExecuting<T>(Func<T> action);
}