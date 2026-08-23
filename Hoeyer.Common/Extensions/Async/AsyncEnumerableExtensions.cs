using System.Collections.Concurrent;

namespace Hoeyer.Common.Extensions.Async;

public static class AsyncEnumerableExtensions
{
    public static async Task<IEnumerable<T>> Collect<T>(this IAsyncEnumerable<T> enumerable)
    {
        var list = new ConcurrentBag<T>();
        await foreach (var item in enumerable)
        {
            list.Add(item);
        }

        return list;
    }
}