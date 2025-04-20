using System;
using System.Collections.Generic;

namespace Hoeyer.Common.Extensions.Types;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue fallBackValue)
    {
        if (dict.TryGetValue(key, out var value))
        {
            return value;
        }

        dict[key] = fallBackValue;
        return fallBackValue;
    }

    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
    {
        if (dict.TryGetValue(key, out var value))
        {
            return value;
        }

        return dict[key] = new TValue();
    }
    
    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> factory)
    {
        if (dict.TryGetValue(key, out var value))
        {
            return value;
        }

        return dict[key] = factory.Invoke();
    }
}