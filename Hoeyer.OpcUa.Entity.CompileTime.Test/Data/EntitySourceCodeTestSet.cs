using FluentResults;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

namespace Hoeyer.OpcUa.Entity.Analysis.Test.Data;

public static class EntitySourceCodeTestSet
{
    internal static readonly IReadOnlyList<EntitySourceCode>
        Valid = LoadSourceCodeDefinitionFor(OpcTestEntities.Valid).ToList();

    internal static readonly IReadOnlyList<EntitySourceCode> Invalid =
        LoadSourceCodeDefinitionFor(OpcTestEntities.Invalid).ToList();

    internal static readonly IReadOnlyList<EntitySourceCode> All = Invalid.Union(Valid).ToList();

    private static IEnumerable<EntitySourceCode> LoadSourceCodeDefinitionFor(IEnumerable<Type> types)
    {
        return types.AsParallel()
            .Select(OpcEntityResourceLoader.LoadTypeAsResourceString)
            .Merge()
            .GetOrThrow(e => new InvalidDataException(e.Message))
            .Select(e => new EntitySourceCode(e.Type, e.TypeAsString));
    }
}