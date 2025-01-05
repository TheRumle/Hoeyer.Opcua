using System.Collections.Immutable;
using System.Reflection;
using Hoeyer.Common.Extensions.Reflection;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public static class OpcTestEntities
{
    public static readonly string CurrentNamespace = typeof(OpcTestEntities).Namespace!;
    
    public static readonly ImmutableHashSet<Type> TestEntities =  Assembly
        .GetExecutingAssembly()
        .GetExportedTypes()
        .Where(type => type.FullName != default && type.FullName.Contains(CurrentNamespace))
        .Where(type => type is { IsAbstract: false, IsInterface: false })
        .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
        .ToImmutableHashSet();
    
    public static ImmutableHashSet<Type> PositiveTestClasses = TestEntities
        .Where(type => type.FullName!.Contains($"{nameof(EntityDefinitions)}.{nameof(Correct)}"))
        .ToImmutableHashSet();
    
    public static ImmutableHashSet<Type> NegativeEntities = TestEntities
        .Where(type => type.FullName!.Contains($"{nameof(EntityDefinitions)}.{nameof(Incorrect)}"))
        .ToImmutableHashSet();
}