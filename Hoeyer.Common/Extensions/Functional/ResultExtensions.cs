using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace Hoeyer.Common.Extensions.Functional;

public static class ResultExtensions
{
    public static Result<IEnumerable<TIn>> Traverse<TIn>(this IEnumerable<Result<TIn>> results, Func<TIn, TIn, TIn> func)
    {
        List<Result<TIn>> enumerable = results.ToList();
        if (enumerable.Exists(r => r.IsSuccess))
        {
            return Result.Ok(enumerable.Select(e=>e.Value));
        }
        return Result.Fail<IEnumerable<TIn>>( enumerable
            .Where(r => r.IsFailed)
            .SelectMany(r => r.Errors));
    }
    
}