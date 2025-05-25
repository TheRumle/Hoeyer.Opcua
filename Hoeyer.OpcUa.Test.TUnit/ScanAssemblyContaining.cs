namespace Hoeyer.Opc.Ua.Test.TUnit;

public static class ScanAssemblyContaining<TAssemblyToken>
{
    public static List<T> GetTypeWithEmptyConstructor<T>()
    {
        return typeof(TAssemblyToken).Assembly
            .GetTypes()
            .Where(t => typeof(T).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null)
            .ToHashSet()
            .Select(analyzerType => (T)Activator.CreateInstance(analyzerType)!)
            .ToList();
    }
}