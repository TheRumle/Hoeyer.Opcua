using System.Collections.Generic;

namespace Hoeyer.Common.Extensions.Types;

public static class ListExtensions
{

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        foreach (var i in items)
        {
            list.Add(i);
        }
    }
    
    
    
}