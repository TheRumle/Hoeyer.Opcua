namespace Hoeyer.Common.Extensions;

public static class StringExtensions
{
    public static string SeparateBy(this IEnumerable<string> strings, string separator) =>
        string.Join(separator, strings);

    public static string SeparateBy<T>(this IEnumerable<T> strings, string separator) =>
        string.Join(separator, strings);

    public static string SeparateBy<T>(this IEnumerable<T> values, string separator, Func<T, string> selector) =>
        values.Select(selector).SeparateBy(separator);

    public static string ToNewlineSeparatedString<T>(this IEnumerable<T> values) => string.Join("\n", values);

    public static string ToCommaSeparatedString<T>(this IEnumerable<T> values) => string.Join(", ", values);
}