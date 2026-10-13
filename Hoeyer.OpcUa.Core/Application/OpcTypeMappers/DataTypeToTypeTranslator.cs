using Hoeyer.OpcUa.Core.Abstractions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public static class DataTypeToTypeTranslator
{
    public static T TranslateToSingle<T>(
        IEntityNode node, string name)
    {
        var p = node.PropertyByBrowseName.GetValueOrDefault(name);
        if (p == null)
        {
            throw new ArgumentException("Unknown property: " + name);
        }

        return (T)OpcToCSharpValueParser.ParseOpcValue(p.Value)!;
    }

    public static TCollection? TranslateToCollection<TCollection, T>(IEntityNode node, string name)
        where TCollection : ICollection<T>, new()
    {
        var p = node.PropertyByBrowseName.GetValueOrDefault(name);
        if (p == null)
        {
            return [];
        }

        T[] collectionValues = p.Value switch
        {
            var v when v.TypeInfo.ValueRank != ValueRanks.Scalar => [OpcToCSharpValueParser.ParseTo<T>(v)],
            var v => OpcToCSharpValueParser.ParseToArray<T>(v),
        };

        return collectionValues.Aggregate(new TCollection(), (current, element) =>
        {
            current.Add(element);
            return current;
        });
    }
}