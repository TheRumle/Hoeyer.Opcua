using TUnit.Assertions.Core;

namespace Hoeyer.OpcUa.IntegrationTest.Extensions;

public sealed class ContainsAssertion<T>(
    AssertionContext<IEnumerable<T>> context,
    T expected) : Assertion<IEnumerable<T>>(context)
{
    protected override Task<AssertionResult> CheckAsync(
        EvaluationMetadata<IEnumerable<T>> metadata)
    {
        if (metadata.Exception is not null)
        {
            return Task.FromResult(
                AssertionResult.Failed(
                    $"threw {metadata.Exception.GetType().Name}"));
        }

        var value = metadata.Value;

        if (value is null)
        {
            return Task.FromResult(
                AssertionResult.Failed("value was null"));
        }

        if (value.Contains(expected))
        {
            return Task.FromResult(AssertionResult.Passed);
        }

        return Task.FromResult(
            AssertionResult.Failed(
                $"'{value}' does not contain '{expected}'"));
    }

    protected override string GetExpectation()
        => $"to contain '{expected}'";
}