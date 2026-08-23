namespace Hoeyer.Common.Extensions.Exceptions;

public static class ExceptionExtensions
{
    public static AggregateException ToAggregateException(this IEnumerable<Exception> exceptions) => new(exceptions);

    public static TResult ToNewlineSeparated<TResult>(this IEnumerable<Exception> exceptions,
        Func<string, TResult> newException) where TResult : Exception =>
        exceptions.ToCharSeparated('\n', newException);

    public static TResult ToCharSeparated<TResult>(this IEnumerable<Exception> exceptions, char character,
        Func<string, TResult> newException) where TResult : Exception
    {
        return newException.Invoke(string.Join($"{character}", exceptions.Select(e => e.Message)));
    }
}