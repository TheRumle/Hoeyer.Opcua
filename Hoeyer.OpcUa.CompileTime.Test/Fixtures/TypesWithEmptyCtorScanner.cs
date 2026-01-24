namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures;

public class TypesWithEmptyCtorScanner<T, TAssemblyToken>
{
    private static readonly IReadOnlyList<T> Generators =
        typeof(TAssemblyToken).Assembly
            .GetTypes()
            .Where(t => typeof(T).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null)
            .ToHashSet()
            .Select(analyzerType => (T)Activator.CreateInstance(analyzerType)!)
            .ToList();

    public IEnumerable<Func<T>> GenerateDataSources()
    {
        return Generators.Select(generator => (Func<T>)(() => generator));
    }
}