using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Server.Entity.Management;

internal class RequestResponseProcessorFactory(LogLevel errorLevel, LogLevel successLevel)
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
        string operationName,
        IEnumerable<T> valuesToProcess,
        Action<T> processSuccess,
        Action<T> processError,
        ILogger logger
    ) where T : IStatusCodeResponse
    {
        return GetProcessorImpl(valuesToProcess, processSuccess, processError, operationName)
            .WithLogging(logger, operationName, errorLevel, successLevel);
    }

    [Pure]
    private static RequestResponseProcessor<T> GetProcessorImpl<T>(
        IEnumerable<T> valuesToProcess,
        Action<T> processSuccess,
        Action<T> processError,
        string? operationName = null
    ) where T : IStatusCodeResponse
    {
        return new RequestResponseProcessor<T>(valuesToProcess, processSuccess, processError, operationName);
    }
}