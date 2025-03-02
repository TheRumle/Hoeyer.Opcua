using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace Hoeyer.Common.Extensions.Types;

public static class ListExtensions
{

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        foreach (var i in items)
        {
            list.Add(i);
        }
    }

    public static (List<TValue> Fit, List<TValue> Fail) WithSuccessCriteria<TValue>(this IEnumerable<TValue> values, Predicate<TValue> success)
    {
        var v = values as TValue[] ?? values.ToArray();
        return (
            Fit: v.Where(e => success(e)).ToList(),
            Fail: v.Where(e => !success(e)).ToList());
    }
    
    
    public static (List<TValue> Fit, List<TValue> Fail) WithSuccessCriteria<TValue>(this IEnumerable<Result<TValue>> values, Predicate<TValue> success)
    {
        var v = values as Result<TValue>[] ?? values.ToArray();
        return (
            Fit: v.Where(e => e.IsSuccess && success(e.Value)).Select(e=>e.Value).ToList(),
            Fail: v.Where(e => e.IsFailed || !success(e.Value)).Select(e=>e.Value).ToList());
    }


    
}