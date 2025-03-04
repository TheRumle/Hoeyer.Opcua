using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Server.Entity.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.RequestResponse;

internal class RequestResponseProcessor<T>(
    IEnumerable<T> valuesToProcess,
    Action<T> markRequestProcessed,
    Action<T> processError
) where T : IStatusCodeResponse, IRequestResponse
{
    private LogLevel _errorLevel;
    private ILogger? _logger;
    private LogLevel _successLevel;

    public RequestResponseProcessor<T> WithLogging(
        ILogger logger,
        LogLevel errorLevel = LogLevel.Error,
        LogLevel successLevel = LogLevel.None)
    {
        _errorLevel = errorLevel;
        _successLevel = successLevel;
        _logger = logger;
        return this;
    }


    public void Process(
        Func<T, string>? errorFormat = null,
        Func<T, string>? successFormat = null)
    {
        var formatError = errorFormat ?? (e => e.ToString());
        var formatSuccess = successFormat ?? (e => e.ToString());

        var (fits, fails) = valuesToProcess
            .Then(markRequestProcessed.Invoke)
            .WithSuccessCriteria(e => e.IsSuccess && StatusCode.IsGood(e.ResponseCode));

        if (_logger != null)
        {
            if (_successLevel != LogLevel.None)
                _logger.Log(_successLevel, "Responses with success: [{@Attributes}]",
                    fits.Select(formatSuccess)
                        .OrderBy(text => text)
                        .SeparateBy(", "));

            if (_errorLevel != LogLevel.None)
                _logger.Log(_successLevel, "There were failing responses: [{@AttributeAndStatus}]",
                    fails
                        .Select(formatError)
                        .OrderBy(text => text)
                        .SeparateBy(", "));
        }

        fails.Then(processError);
    }
}