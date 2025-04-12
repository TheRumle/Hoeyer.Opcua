namespace Hoeyer.Opc.Ua.Test.TUnit.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<Func<T>> SelectFunc<T>(this IEnumerable<T> source)
    {
        return source.Select(e => new Func<T>(() => e));
    }

    public static IEnumerable<Func<TResult>> SelectFunc<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, TResult> selector)
    {
        return source.Select(original => new Func<TResult>(() => selector.Invoke(original)));
    }
}