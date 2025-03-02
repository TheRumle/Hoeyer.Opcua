using System.Collections.Generic;

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
}