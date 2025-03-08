using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Server.Entity.Management;

internal class RequestResponseProcessorFactory(LogLevel errorLevel, LogLevel successLevel)
{
    public IRequestResponseProcessor<T> GetProcessor<T>(
        IEnumerable<T> valuesToProcess,
        Action<T> processSuccess,
        Action<T> processError
        ) where T : IStatusCodeResponse
    {
        return GetProcessorImpl(valuesToProcess, processSuccess, processError);
    }
    
    public IRequestResponseProcessor<T> GetProcessorWithLoggingFor<T>(
        string operationName,
        IEnumerable<T> valuesToProcess,
        Action<T> processSuccess,
        Action<T> processError,
        ILogger logger
    ) where T : IStatusCodeResponse
    {
        return GetProcessorImpl(valuesToProcess, processSuccess, processError, operationName).WithLogging(logger, operationName, errorLevel, successLevel);
    }
    
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