using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.Extensions;

public static class TestContextExtensions
{
    private static ClassDataSourceAttribute<T>? ExtractUsedClassDataSource<T>(this ITestMetadata? metadata)
    {
        if (metadata?.MethodDataSource is ClassDataSourceAttribute<T> methodSource)
        {
            return methodSource;
        }

        if (metadata?.ClassDataSource is ClassDataSourceAttribute<T> classDataSource)
        {
            return classDataSource;
        }

        return null;
    }


    public static string ComputeKey<T>(TestContext? context)
    {
        var metadata = context?.Metadata!;
        var usedDataSource = metadata.ExtractUsedClassDataSource<T>();

        if (usedDataSource == null)
        {
            throw new Exception($"Could not extract {nameof(ClassDataSourceAttribute)} from {typeof(T).Name}");
        }

        return (usedDataSource?.Shared, usedDataSource?.Key).ExtractAdapterKey(
            metadata.TestDetails.ClassType.Name,
            metadata.TestDetails.TestName
        );
    }

    public static string ExtractAdapterKey(this (SharedType? Shared, string? Key) pair, string perClassName,
        string perTestName)
    {
        return (pair.Shared, pair.Key) switch
        {
            (null, null) => TestKeys.PerTestSessionKey,
            (SharedType.PerTestSession, _) => TestKeys.PerTestSessionKey,
            (SharedType.Keyed, var k) => k!,
            (SharedType.PerClass, var _) => perClassName,
            (SharedType.PerAssembly, var _) => TestKeys.PER_ASSEMBLY_KEY,
            (SharedType.None, var _) => perTestName,
            var _ => throw new ArgumentOutOfRangeException(nameof(pair), pair, null)
        };
    }
}