using System;
using System.Collections;
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
            .Select(e=>e.Result)));
    }
    
    public static Task<Result<T>> Traverse<T>(this Task<T> task)
    {
        return Result.Try(() => task);
    }
    
    public static Task<Result<T>> Traverse<T>(this Task<T> task, Func<Exception, Error> onError)
    {
        return Result.Try(() => task, onError);
    }
    
    /// <summary>
    /// Tap into the result and perform a side effects, and return the original result!
    /// </summary>
    /// <param name="result">The result which is used to propagate values</param>
    /// <param name="onSuccess">What will happen if the result is successful</param>
    /// <param name="onError">What will happen if the result has errored</param>
    /// <typeparam name="T">The type of the value the result can hold</typeparam>
    /// <returns>The original result without modification</returns>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> onSuccess,  Action<List<IError>> onError)
    {
        if (result.IsSuccess) onSuccess(result.Value);
        else onError(result.Errors);
        return result;
    }
    
   
    

}