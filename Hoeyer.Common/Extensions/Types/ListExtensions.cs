using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.Common.Extensions.Types;

public static class ListExtensions
{
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        foreach (var i in items) list.Add(i);
    }

    public static (List<TValue> Fit, List<TValue> Fail) WithSuccessCriteria<TValue>(this IEnumerable<TValue> values,
        Predicate<TValue> success)
    {
        var v = values as TValue[] ?? values.ToArray();
        return (
            Fit: v.Where(e => success(e)).ToList(),
            Fail: v.Where(e => !success(e)).ToList());
    }


    public static (IEnumerable<T> Matching, IEnumerable<T> NonMatching) PartitionBy<T>(
        this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var matching = new List<T>();
        var nonMatching = new List<T>();

        foreach (var item in source)
            if (predicate(item))
            {
                matching.Add(item);
            }
            else
            {
                nonMatching.Add(item);
            }

        return (matching, nonMatching);
    }
}