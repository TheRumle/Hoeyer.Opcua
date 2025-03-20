using System.Diagnostics.CodeAnalysis;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Fixtures.Generator;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public class TypesWithEmptyCtorScanningGeneratorAttribute<T, TAssemblyToken> : DataSourceGeneratorAttribute<T>
{
    private static readonly IReadOnlyList<T> Generators = GetTypes();

    public sealed override IEnumerable<Func<T>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return Generators.Select(generator => (Func<T>)(() => generator));
    }
    private static List<T> GetTypes()
    {
        return typeof(TAssemblyToken).Assembly
            .GetTypes()
            .Where(t => typeof(T).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null)
            .ToHashSet()
            .Select(analyzerType => (T)Activator.CreateInstance(analyzerType)!)
            .ToList();
    }
}