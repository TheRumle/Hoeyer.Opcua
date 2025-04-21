using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Core.Application.RequestResponse;

public class StatusCodeResponseProcessorFactory(LogLevel errorLevel, LogLevel successLevel, ILogger logger)
{
    [SuppressMessage("Maintainability", "S2325",
        Justification =
            "The factory should not have static methods as it abstracts away dependencies for object creation")]
    public IRequestResponseProcessor<T> GetProcessor<T>(
        IEnumerable<T> valuesToProcess,
        Action<T> processSuccess,
        Action<T> processError
    ) where T : IStatusCodeResponse
    {
        return GetProcessorImpl(valuesToProcess, processSuccess, processError);
    }

    [Pure]
    public IRequestResponseProcessor<T> GetProcessorWithLoggingFor<T>(
        IEnumerable<T> valuesToProcess,
        Action<T> processSuccess,
        Action<T> processError
    ) where T : IStatusCodeResponse
    {
        return GetProcessorImpl(valuesToProcess, processSuccess, processError)
            .WithLogging(logger, errorLevel, successLevel);
    }
    
    [Pure]
    public IRequestResponseProcessor<T> GetProcessorWithLoggingForFailedOnly<T>(IEnumerable<T> valuesToProcess,
        Action<T> processSuccess,
        Action<T> processError,
        ILogger logger
    ) where T : IStatusCodeResponse
    {
        return GetProcessorImpl(valuesToProcess, processSuccess, processError)
            .WithLogging(logger, errorLevel);
    }

    [Pure]
    private static RequestResponseProcessor<T> GetProcessorImpl<T>(
        IEnumerable<T> valuesToProcess,
        Action<T> processSuccess,
        Action<T> processError) where T : IStatusCodeResponse
    {
        return new RequestResponseProcessor<T>(valuesToProcess, processSuccess, processError);
    }
}