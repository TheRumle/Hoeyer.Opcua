using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hoeyer.Common.Extensions.Async;

public static class AsyncEnumerableExtensions
{
    public static async Task<IEnumerable<T>> Collect<T>(this IAsyncEnumerable<T> enumerable)
    {
        var list = new List<T>();
        await foreach (var item in enumerable)
        {
            list.Add(item);
        }

        return list;
    }
    
}