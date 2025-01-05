using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.Common.Extensions;

public static class StringExtensions
{
    public static string SeparatedBy(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

    public static string SeparatedBy<T>(this IEnumerable<T> values, string separator, Func<T, string> selector) => SeparatedBy(values.Select(selector), separator);
}