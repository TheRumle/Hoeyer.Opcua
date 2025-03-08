using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.Common.Extensions.Types;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;

public interface IRequestResponseProcessor<T> where T : IStatusCodeResponse, IRequestResponse
{
    /// <summary>
    /// Begin processing of the values.
    /// </summary>
    /// <param name="errorFormat">How to format error messages.</param>
    /// <param name="errorFilter">A filter for which errors to include</param>
    /// <param name="successFormat">How to format successMessages</param>
    void Process(
        Func<T, string>? errorFormat = null,
        Predicate<T> errorFilter = null,
        Func<T, string>? successFormat = null
        );
}

internal class RequestResponseProcessor<T>(
    IEnumerable<T> valuesToProcess,
    Action<T> processSuccess,
    Action<T> processError,
    string? operationName = null
) : IRequestResponseProcessor<T> where T : IStatusCodeResponse, IRequestResponse
{
    private LogLevel _errorLevel;
    private ILogger? _logger;
    private LogLevel _successLevel;
    private string? _operationName = operationName;

    public RequestResponseProcessor<T> WithLogging(
        ILogger logger,
        string operationName,
        LogLevel errorLevel = LogLevel.Error,
        LogLevel successLevel = LogLevel.None)
    {
        _errorLevel = errorLevel;
        _successLevel = successLevel;
        _logger = logger;
        _operationName = operationName;
        return this;
    }

    /// <inheritdoc/>
    public void Process(
        Func<T, string>? errorFormat = null,
        Predicate<T> errorFilter = null,
        Func<T, string>? successFormat = null)
    {
        var formatError = errorFormat ?? (e => e.ToString());
        var errorPred = errorFilter ?? (e => true);
        var formatSuccess = successFormat ?? (e => e.ToString());
        
        var (fits, fails) = valuesToProcess
            .Then(processSuccess.Invoke)
            .WithSuccessCriteria(e => e.IsSuccess && StatusCode.IsGood(e.ResponseCode));
        
        var failures = fails.FindAll(errorPred);

        if (_logger != null)
        {
            if (_successLevel != LogLevel.None)
                LogSuccess(fits, formatSuccess);

            if (_errorLevel != LogLevel.None && failures.Any())
                LogErrors(failures, formatError);
        }
        failures.Then(processError);
    }

    private void LogSuccess(List<T> fits, Func<T, string> formatSuccess)
    {
        _logger.Log(_successLevel, "{OperationName} : [{@Attributes}]",
            _operationName,
            fits.Select(formatSuccess)
                .OrderBy(text => text)
                .SeparateBy(", "));
    }

    private void LogErrors(List<T> fails, Func<T, string> formatError)
    {
        
        _logger.Log(_errorLevel, "Failed {OperationName} : [{@AttributeAndStatus}]",
            _operationName,
            fails.Select(formatError)
                .OrderBy(text => text)
                .SeparateBy(", "));
    }
}