using System.Collections.Immutable;
using Hoeyer.Common.Extensions.Reflection;
using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public static class OpcTestEntities
{
    public static readonly string CurrentNamespace = typeof(OpcTestEntities).Namespace!;
    public static readonly string CurrentPath = CurrentNamespace.Replace('.', Path.DirectorySeparatorChar);

    public static readonly ImmutableHashSet<Type> All = typeof(OpcTestEntities).Assembly
        .GetTypes()
        .Where(type => type.FullName != default && type.FullName.Contains(CurrentNamespace))
        .Where(type => type is { IsAbstract: false, IsInterface: false })
        .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
        .ToImmutableHashSet();

    public static readonly ImmutableHashSet<Type> Valid = All
        .Where(type => type.FullName!.Contains($"{nameof(EntityDefinitions)}.Correct"))
        .ToImmutableHashSet();

    public static readonly ImmutableHashSet<Type> PropertyAccessViolations = All
        .Where(type =>
            type.FullName!.Contains(
                $"{nameof(EntityDefinitions)}.{nameof(Incorrect)}.{nameof(Incorrect.PropertyAccess)}"))
        .ToImmutableHashSet();
}