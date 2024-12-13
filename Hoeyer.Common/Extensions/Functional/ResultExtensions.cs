﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;

namespace Hoeyer.Common.Extensions.Functional;

public static class ResultExtensions
{

    public static Task<Result<IEnumerable<T>>> Traverse<T>(this IEnumerable<Task<T>> tasks)
    {   
        return Task.FromResult(Result.Try(() => tasks.Select(async e => await e)
            .Select(e=>e.Result)));
    }
    

    public static Result<T> Tap<T>(this Result<T> result, Action<T> onSuccess,  Action<List<IError>> onError)
    {
        if (result.IsSuccess) onSuccess(result.Value);
        else onError(result.Errors);
        return result;
    }

}