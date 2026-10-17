using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions;

public static class ArrayOfExtensions
{
    private static T[] ToArraySafe<T>(this ArrayOf<T> values)
        => values.ToArray() ?? [];

    extension<T>(ArrayOf<T> values)
    {
        // Projection
        public IEnumerable<TResult> Select<TResult>(
            Func<T, TResult> selector)
            => values.ToArraySafe().Select(selector);

        public IEnumerable<TResult> Select<TResult>(
            Func<T, int, TResult> selector)
            => values.ToArraySafe().Select(selector);

        // Filtering
        public IEnumerable<T> Where(
            Func<T, bool> predicate)
            => values.ToArraySafe().Where(predicate);

        public IEnumerable<T> Where(
            Func<T, int, bool> predicate)
            => values.ToArraySafe().Where(predicate);

        // Flattening
        public IEnumerable<TResult> SelectMany<TResult>(
            Func<T, IEnumerable<TResult>> selector)
            => values.ToArraySafe().SelectMany(selector);

        public IEnumerable<TResult> SelectMany<TResult>(
            Func<T, int, IEnumerable<TResult>> selector)
            => values.ToArraySafe().SelectMany(selector);

        public IEnumerable<TResult> SelectMany<TCollection, TResult>(
            Func<T, IEnumerable<TCollection>> collectionSelector,
            Func<T, TCollection, TResult> resultSelector)
            => values.ToArraySafe().SelectMany(collectionSelector, resultSelector);

        // Ordering
        public IOrderedEnumerable<T> OrderBy<TKey>(
            Func<T, TKey> keySelector)
            => values.ToArraySafe().OrderBy(keySelector);

        public IOrderedEnumerable<T> OrderByDescending<TKey>(
            Func<T, TKey> keySelector)
            => values.ToArraySafe().OrderByDescending(keySelector);

        public IOrderedEnumerable<T> ThenBy<TKey>(
            Func<T, TKey> keySelector)
            => values.ToArraySafe().OrderBy(keySelector);

        public IOrderedEnumerable<T> ThenByDescending<TKey>(
            Func<T, TKey> keySelector)
            => values.ToArraySafe().OrderByDescending(keySelector);

        // Quantifiers
        public bool Any()
            => values.ToArraySafe().Any();

        public bool Any(
            Func<T, bool> predicate)
            => values.ToArraySafe().Any(predicate);

        public bool All(
            Func<T, bool> predicate)
            => values.ToArraySafe().All(predicate);

        public bool Contains(T value)
            => values.ToArraySafe().Contains(value);

        // Counts
        public int Count()
            => values.ToArraySafe().Length;

        public int Count(
            Func<T, bool> predicate)
            => values.ToArraySafe().Count(predicate);

        public long LongCount()
            => values.ToArraySafe().LongCount();

        public long LongCount(
            Func<T, bool> predicate)
            => values.ToArraySafe().LongCount(predicate);

        // Element access
        public T First()
            => values.ToArraySafe()[0] ?? throw new InvalidOperationException("Sequence contains no elements");

        public T First(
            Func<T, bool> predicate)
            => values.ToArraySafe().First(predicate);

        public T? FirstOrDefault()
            => values.ToArraySafe().FirstOrDefault();

        public T? FirstOrDefault(
            Func<T, bool> predicate)
            => values.ToArraySafe().FirstOrDefault(predicate);

        public T Last() => values.ToArraySafe()[^1];

        public T Last(
            Func<T, bool> predicate)
            => values.ToArraySafe().Last(predicate);

        public T? LastOrDefault()
            => values.ToArraySafe().LastOrDefault();

        public T Single()
            => values.ToArraySafe().Single();

        public T Single(
            Func<T, bool> predicate)
            => values.ToArraySafe().Single(predicate);

        public T ElementAt(int index)
            => values.ToArraySafe()[index];

        // Defaulting
        public IEnumerable<T?> DefaultIfEmpty()
            => values.ToArraySafe().DefaultIfEmpty();

        public IEnumerable<T> DefaultIfEmpty(T defaultValue)
            => values.ToArraySafe().DefaultIfEmpty(defaultValue);

        // Set operations
        public IEnumerable<T> Distinct()
            => values.ToArraySafe().Distinct();

        public IEnumerable<T> Union(IEnumerable<T> second)
            => values.ToArraySafe().Union(second);

        public IEnumerable<T> Intersect(IEnumerable<T> second)
            => values.ToArraySafe().Intersect(second);

        public IEnumerable<T> Except(IEnumerable<T> second)
            => values.ToArraySafe().Except(second);

        // Partitioning
        public IEnumerable<T> Take(int count)
            => values.ToArraySafe().Take(count);

        public IEnumerable<T> Skip(int count)
            => values.ToArraySafe().Skip(count);

        public IEnumerable<T> TakeWhile(
            Func<T, bool> predicate)
            => values.ToArraySafe().TakeWhile(predicate);

        public IEnumerable<T> SkipWhile(
            Func<T, bool> predicate)
            => values.ToArraySafe().SkipWhile(predicate);

        // Aggregation
        public T Aggregate(
            Func<T, T, T> func)
            => values.ToArraySafe().Aggregate(func);

        public TAccumulate Aggregate<TAccumulate>(
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> func)
            => values.ToArraySafe().Aggregate(seed, func);

        // Materialization
        public List<T> ToList()
            => values.ToArraySafe().ToList();

        // Joining
        public string Join(string separator)
            => string.Join(separator, values.ToArraySafe());
    }
}