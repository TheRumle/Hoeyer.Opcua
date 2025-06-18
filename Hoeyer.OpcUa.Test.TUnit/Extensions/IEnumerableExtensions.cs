namespace Hoeyer.Opc.Ua.Test.TUnit.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<Func<TOther>> Map<T, TOther>(this IEnumerable<Func<T>> source, Func<T, TOther> selector)
    {
        return source.SelectFunc(e => selector.Invoke(e.Invoke()));
    }

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