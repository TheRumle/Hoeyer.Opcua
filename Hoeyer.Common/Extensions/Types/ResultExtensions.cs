using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace Hoeyer.Common.Extensions.Types;

public static class ResultExtensions
{
    /// <summary>
    ///     Pipes the values of the result into a the action and returns the original result unmodified
    /// </summary>
    /// <param name="result">A result containing a <typeparamref name="T" /> or errors</param>
    /// <param name="action">The action to perform only if <paramref name="result" /> is successful</param>
    /// <typeparam name="T">The type of the content of the result</typeparam>
    /// <returns>The original result</returns>
    public static Result<T> Then<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    public static IEnumerable<T> Then<T>(this IEnumerable<T> result, Action<T> stateChanger)
    {
        var enumerable = result as T[] ?? result.ToArray();
        foreach (var r in enumerable) stateChanger.Invoke(r);

        return enumerable;
    }


    public static Result<(TLeft Left, TRight Right)> MergeWith<TLeft, TRight>(this Result<TLeft> left,
        Result<TRight> right)
    {
        if (left.IsFailed || right.IsFailed)
        {
            return Result.Fail(left.Errors.Union(right.Errors));
        }

        return (left.Value, right.Value);
    }
}