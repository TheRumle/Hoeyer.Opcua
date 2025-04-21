using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.Common.Extensions.Types;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Core.Application.RequestResponse;

public class RequestResponseProcessor<T>(
    IEnumerable<T> valuesToProcess,
    Action<T> processSuccess,
    Action<T> processError) : IRequestResponseProcessor<T> where T : IRequestResponse
{
    private LogLevel _errorLevel;
    private ILogger? _logger;
    private LogLevel _successLevel;

    /// <inheritdoc />
    public void Process(
        Predicate<T>? additionalSuccessCriteria = null,
        Func<T, string>? errorFormat = null,
        Func<T, string>? successFormat = null)
    {
        var formatError = errorFormat ?? (e => e.ToString());
        var formatSuccess = successFormat ?? (e => e.ToString());
        var successFilter = additionalSuccessCriteria ?? (e => e.IsSuccess);

        var (fits, fails) = valuesToProcess
            .Then(processSuccess.Invoke)
            .WithSuccessCriteria(successFilter);

        if (_successLevel != LogLevel.None)
        {
            LogSuccess(fits, formatSuccess);
        }

        if (_errorLevel != LogLevel.None && fails.Any())
        {
            LogErrors(fails, formatError);
        }

        fails.Then(processError);
    }

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

    private void LogSuccess(List<T> fits, Func<T, string> formatSuccess)
    {
        _logger?.Log(_successLevel, "Success: [{@Attributes}]",
            fits.Select(formatSuccess)
                .OrderBy(text => text)
                .SeparateBy(", "));
    }

    private void LogErrors(List<T> fails, Func<T, string> formatError)
    {
        _logger?.Log(_errorLevel, "Failed: [{@AttributeAndStatus}]",
            fails.Select(formatError)
                .OrderBy(text => text)
                .SeparateBy(", "));
    }
}