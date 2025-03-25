namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures;

public abstract class TypesWithEmptyCtorScanningGeneratorAttribute<T, TAssemblyToken> : DataSourceGeneratorAttribute<T>
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