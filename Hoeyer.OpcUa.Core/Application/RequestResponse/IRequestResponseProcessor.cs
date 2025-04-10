using System;

namespace Hoeyer.OpcUa.Core.Application.RequestResponse;

public interface IRequestResponseProcessor<out T>
{
    /// <summary>
    ///     Begin processing of the values.
    /// </summary>
    /// <param name="additionalSuccessCriteria">A predicate that dictates whether the response is 'good' or 'bad'</param>
    /// <param name="errorFormat">How to format error messages.</param>
    /// <param name="successFormat">How to format successMessages</param>
    void Process(
        Predicate<T>? additionalSuccessCriteria = null,
        Func<T, string>? errorFormat = null,
        Func<T, string>? successFormat = null
    );
}