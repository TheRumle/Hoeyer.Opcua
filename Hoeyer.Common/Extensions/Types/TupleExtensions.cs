using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.Common.Extensions.Types;

public static class TupleExtensions
{
    // For a tuple (T, T)
    public static IEnumerable<T> ToEnumerable<T>(this (T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
    }

    // For a tuple (T, T, T)
    public static IEnumerable<T> ToEnumerable<T>(this (T, T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
    }

    // For a tuple (T, T, T, T)
    public static IEnumerable<T> ToEnumerable<T>(this (T, T, T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
    }

    // For a tuple (T, T, T, T, T)
    public static IEnumerable<T> ToEnumerable<T>(this (T, T, T, T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
    }

    // For a tuple (T, T, T, T, T, T)
    public static IEnumerable<T> ToEnumerable<T>(this (T, T, T, T, T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
        yield return tuple.Item6;
    }

    // For a tuple (T, T, T, T, T, T, T)
    public static IEnumerable<T> ToEnumerable<T>(this (T, T, T, T, T, T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
        yield return tuple.Item6;
        yield return tuple.Item7;
    }

    // For a tuple (T, T, T, T, T, T, T, T)
    public static IEnumerable<T> ToEnumerable<T>(this (T, T, T, T, T, T, T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
        yield return tuple.Item6;
        yield return tuple.Item7;
        yield return tuple.Item8;
    }

    // For a tuple (T, T, T, T, T, T, T, T, T)
    public static IEnumerable<T> ToEnumerable<T>(this (T, T, T, T, T, T, T, T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
        yield return tuple.Item6;
        yield return tuple.Item7;
        yield return tuple.Item8;
        yield return tuple.Item9;
    }

    // For a tuple (T, T, T, T, T, T, T, T, T, T)
    public static IEnumerable<T> ToEnumerable<T>(this (T, T, T, T, T, T, T, T, T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
        yield return tuple.Item6;
        yield return tuple.Item7;
        yield return tuple.Item8;
        yield return tuple.Item9;
        yield return tuple.Item10;
    }

    public static IEnumerable<(TFirst first, TSecond second)> Zip<TFirst, TSecond>(
        this (IEnumerable<TFirst> first, IEnumerable<TSecond> second) elements)
    {
        var firstList = elements.first.ToList();
        var secondList = elements.second.ToList();
        if (firstList.Count != secondList.Count)
        {
            throw new ArgumentException("The input sequences must have the same length.");
        }

        return GetZipIterator(firstList, secondList);
    }

    private static IEnumerable<(TFirst first, TSecond second)> GetZipIterator<TFirst, TSecond>(List<TFirst> firstList, List<TSecond> secondList)
    {
        for (var i = 0; i < firstList.Count; i++) yield return (firstList[i], secondList[i]);
    }
}