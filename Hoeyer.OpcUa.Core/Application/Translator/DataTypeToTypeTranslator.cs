using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.Translator.Parsers;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.Translator;

public static class DataTypeToTypeTranslator
{
    public static T? TranslateToSingle<T>(
        IEntityNode node, string name)
    {
        var p = node.PropertyByBrowseName.TryGetValue(name, out var value) ? value : null;
        if (p == null)
        {
            return default;
        }

        return (T)OpcToCSharpValueParser.ParseOpcValue(p.WrappedValue)!;
    }

    public static TCollection? TranslateToCollection<TCollection, T>(IEntityNode node, string name)
        where TCollection : ICollection<T>, new()
    {
        var p = node.PropertyByBrowseName.TryGetValue(name, out var value) ? value : null;
        if (p == null)
        {
            return [];
        }

        var res = new PropertyValueCollectionParser<T>().Parse(p);
        if (res == null)
        {
            return [];
        }

        return res.Aggregate(new TCollection(), (current, element) =>
        {
            current.Add(element);
            return current;
        });
    }

    public static T[] TranslateToArray<T>(IEntityNode node, string name)
    {
        PropertyState? p = node.PropertyByBrowseName.TryGetValue(name, out PropertyState? value) ? value : null;
        if (p == null) return [];

        T[]? res = new PropertyValueCollectionParser<T>().Parse(p);

        if (res == null) return [];

        return res.Aggregate(new List<T>(), (current, element) =>
        {
            current.Add(element);
            return current;
        }).ToArray();
    }
}