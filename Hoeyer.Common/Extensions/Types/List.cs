using System;
using System.Collections.Generic;
using System.Linq;

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

    public static (List<TValue> Fit, List<TValue> Fail) SplitBy<TValue>(this IEnumerable<TValue> values, Predicate<TValue> success)
    {
        var v = values as TValue[] ?? values.ToArray();
        return (
            Fit: v.Where(e => success(e)).ToList(),
            Fail: v.Where(e => !success(e)).ToList());
    }
    
    
    
}