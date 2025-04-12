using Hoeyer.Opc.Ua.Test.TUnit;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures;

public abstract class TypesWithEmptyCtorScanningGeneratorAttribute<T, TAssemblyToken> : DataSourceGeneratorAttribute<T>
{
    private static readonly IReadOnlyList<T> Generators = ScanAssemblyContaining<TAssemblyToken>.GetTypeWithEmptyConstructor<T>();

    public sealed override IEnumerable<Func<T>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return Generators.Select(generator => (Func<T>)(() => generator));
    }
}