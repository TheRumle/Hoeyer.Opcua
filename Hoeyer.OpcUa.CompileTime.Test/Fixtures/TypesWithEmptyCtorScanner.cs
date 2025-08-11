using Hoeyer.Opc.Ua.Test.TUnit;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures;

public class TypesWithEmptyCtorScanner<T, TAssemblyToken>
{
    private static readonly IReadOnlyList<T> Generators =
        ScanAssemblyContaining<TAssemblyToken>.GetTypeWithEmptyConstructor<T>();

    public IEnumerable<Func<T>> GenerateDataSources()
    {
        return Generators.Select(generator => (Func<T>)(() => generator));
    }
}