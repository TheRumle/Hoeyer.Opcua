using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.Common.Extensions.Types;

public static class ResultExtensions
{
    public static IEnumerable<T> Then<T>(this IEnumerable<T> result, Action<T> stateChanger)
    {
        var enumerable = result as T[] ?? result.ToArray();
        foreach (var r in enumerable) stateChanger.Invoke(r);

        return enumerable;
    }
}