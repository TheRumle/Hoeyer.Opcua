using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.Translator.Parsers;

namespace Hoeyer.OpcUa.Core.Application.Translator;

public static class DataTypeToTranslator
{
    public static T? TranslateToSingle<T>(
        IEntityNode node, string name)
    {
        var p = node.PropertyByBrowseName.TryGetValue(name, out var value) ? value : null;
        if (p == null)
        {
            return default;
        }

        var parser = new PropertyValueParser<T>();
        return parser.Parse(p);
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
}