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

    public static Task<Result<IEnumerable<T>>> Traverse<T>(this IEnumerable<Task<T>> tasks)
    {
        return Task.FromResult(Result.Try(() => tasks.Select(async e => await e)
            .Select(e => e.Result)));
    }

    public static Task<Result<T>> Traverse<T>(this Task<T> task)
    {
        return Result.Try(() => task);
    }

    public static Task<Result<T>> Traverse<T>(this Task<T> task, Func<Exception, Error> onError)
    {
        return Result.Try(() => task, onError);
    }

    public static Task<Result<T>> Traverse<T>(this Result<Task<T>> result)
    {
        if (result.IsFailed) return Task.FromResult(Result.Fail<T>(result.Errors));
        return Task.FromResult(result.Bind(e => Result.Try(() => e.Result)));
    }

    public static Task<Result<IEnumerable<T>>> Combine<T>(this IEnumerable<Task<Result<T>>> tasks)
    {
        return Task.WhenAll(tasks).ContinueWith(task => task.Result.AsEnumerable().Merge());
    }

    public static T GetOrThrow<T>(this Result<T> result, Func<Error, Exception> onError)
    {
        if (result.IsSuccess) return result.Value;
        throw onError.Invoke(new Error(result.Errors.SeparatedBy("\n")));
    }


    /// <summary>
    ///     Tap into the result and perform a side effects, and return the original result!
    /// </summary>
    /// <param name="result">The result which is used to propagate values</param>
    /// <param name="onSuccess">What will happen if the result is successful</param>
    /// <param name="onError">What will happen if the result has errored</param>
    /// <typeparam name="T">The type of the value the result can hold</typeparam>
    /// <returns>The original result without modification</returns>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> onSuccess, Action<List<IError>> onError)
    {
        if (result.IsSuccess) onSuccess(result.Value);
        else onError(result.Errors);
        return result;
    }

    public static Result<T> FailIf<T>(this T value, bool check, IError error)
    {
        return check ? value.ToResult() : Result.Fail(error);
    }

    public static Result<T> FailIf<T>(this T value, bool check, string error)
    {
        return value.FailIf(check, new Error(error));
    }

    public static Result<T> FailIf<T>(this T value, Predicate<T> check, string error)
    {
        return value.FailIf(check.Invoke(value), new Error(error));
    }

    public static Result<T> FailIf<T>(this Result<T> value, bool check, IError error)
    {
        if (!check)
            return !value.IsSuccess
                ? Result.Fail(value.Errors.Concat([new Error(error.Message)]))
                : Result.Fail(error);

        return value;
    }

    public static Result<T> FailIf<T>(this Result<T> value, bool check, string error)
    {
        return value.FailIf(check, new Error(error));
    }

    public static Result ToFailedResult(this IError er)
    {
        return Result.Fail(er);
    }

    public static Result<T> ToFailedResult<T>(this IError er)
    {
        return Result.Fail(er);
    }
}