using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public static class DataTypeToTypeTranslator
{
    public static T? TranslateToSingle<T>(
        IAgent node, string name)
    {
        var p = node.PropertyByBrowseName.TryGetValue(name, out var value) ? value : null;
        if (p == null)
        {
            throw new ArgumentException("Unknown property: " + name);
        }

        return (T)OpcToCSharpValueParser.ParseOpcValue(p.WrappedValue)!;
    }

    public static TCollection? TranslateToCollection<TCollection, T>(IAgent node, string name)
        where TCollection : ICollection<T>, new()
    {
        var p = node.PropertyByBrowseName.GetValueOrDefault(name);
        if (p == null)
        {
            return [];
        }

        var collectionValues = p.Value switch
        {
            Variant v => OpcToCSharpValueParser.ParseToArray<T>(v),
            T singleton => [singleton],
            IEnumerable<T> enumerable => enumerable.ToArray(),
            var _ => throw new ArgumentException($"The specified value type '{p.Value.GetType()}' is not supported:")
        };

        return collectionValues.Aggregate(new TCollection(), (current, element) =>
        {
            current.Add(element);
            return current;
        });
    }
}