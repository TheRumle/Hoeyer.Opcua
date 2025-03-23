using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;

namespace Hoeyer.Common.Extensions.Functional;

public static class ResultExtensions
{
    public static Task<Result<IEnumerable<T>>> Traverse<T>(this IEnumerable<Task<Result<T>>> tasks)
    {
        return Task.WhenAll(tasks).ContinueWith(task => task.Result.AsEnumerable().Merge());
    }

    public static Task<Result<T>> Traverse<T>(this Task<T> task, Func<Exception, string> onError)
    {
        return Result.Try(() => task, e => new Error(onError.Invoke(e)));
    }

    public static Task<IEnumerable<Result<T>>> TraverseEach<T>(this IEnumerable<Task<Result<T>>> tasks)
    {
        return Task.WhenAll(tasks).ContinueWith(task =>
        {
            var results = task.Result.ToList();
            return results.AsEnumerable();
        });
    }


    /// <summary>
    ///     If the result is not successful will throw a hard error!
    /// </summary>
    /// <param name="result">The result, which might have been failed</param>
    /// <param name="exceptionFactory">How to create the exception</param>
    /// <returns>The value of the result, if successful</returns>
    /// <exception cref="Exception"></exception>
    public static T GetOrThrow<T>(this Result<T> result, Func<Error, Exception> exceptionFactory)
    {
        if (result.IsSuccess) return result.Value;
        throw exceptionFactory.Invoke(new Error(result.Errors.SeparateBy("\n")));
    }


    /// <summary>
    ///     Tap into the result and perform a side effects, and return the original result!
    /// </summary>
    /// <param name="result">The result which is used to propagate values</param>
    /// <param name="onSuccess">What will happen if the result is successful</param>
    /// <param name="onError">What will happen if the result has errored</param>
    /// <typeparam name="T">The type of the value the result can hold</typeparam>
    /// <returns>The original result without modification</returns>
    public static Result<T> Then<T>(this Result<T> result, Action<T> onSuccess, Action<List<IError>> onError)
    {
        if (result.IsSuccess)
        {
            onSuccess(result.Value);
            return result;
        }

        onError.Invoke(result.Errors);
        return result;
    }

    /// <summary>
    ///     Pipes the values of the result into a the action and returns the original result unmodified
    /// </summary>
    /// <param name="result">A result containing a <typeparamref name="T" /> or errors</param>
    /// <param name="action">The action to perform only if <paramref name="result" /> is successful</param>
    /// <typeparam name="T">The type of the content of the result</typeparam>
    /// <returns>The original result</returns>
    public static Result<T> Then<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess) action(result.Value);
        return result;
    }

    public static IEnumerable<T> Then<T>(this IEnumerable<T> result, Action<T> stateChanger)
    {
        var enumerable = result as T[] ?? result.ToArray();
        foreach (var r in enumerable) stateChanger.Invoke(r);

        return enumerable;
    }
}