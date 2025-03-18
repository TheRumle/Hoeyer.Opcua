using FluentResults;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Data;

public static class EntitySourceCodeTestSet
{
    internal static readonly IReadOnlyList<EntitySourceCode>
        Valid = LoadSourceCodeDefinitionFor(OpcTestEntities.Valid).ToList();

    internal static readonly IReadOnlyList<EntitySourceCode> PropertyAccessViolations =
        LoadSourceCodeDefinitionFor(OpcTestEntities.PropertyAccessViolations).ToList();

    internal static readonly IReadOnlyList<EntitySourceCode> All = PropertyAccessViolations.Union(Valid).ToList();

    private static IEnumerable<EntitySourceCode> LoadSourceCodeDefinitionFor(IEnumerable<Type> types)
    {
        return types.AsParallel()
            .Select(OpcEntityResourceLoader.LoadTypeAsResourceString)
            .Merge()
            .GetOrThrow(e => new InvalidDataException(e.Message))
            .Select(e => new EntitySourceCode(e.Type, e.TypeAsString));
    }
}