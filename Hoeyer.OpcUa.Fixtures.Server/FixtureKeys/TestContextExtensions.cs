using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Fixtures.Server.FixtureKeys;

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
            throw new InvalidOperationException(
                $"Could not extract {nameof(ClassDataSourceAttribute)} from {typeof(T).Name}");
        }

        return ServerFixtureKeyExtractor.ExtractFixtureKey(
            usedDataSource.Shared,
            usedDataSource.Key,
            metadata.TestDetails.ClassType.Name,
            metadata.TestDetails.TestName
        );
    }
}