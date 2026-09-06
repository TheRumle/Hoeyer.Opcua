using TUnit.Assertions.Core;

namespace Hoeyer.OpcUa.IntegrationTest.Extensions;

public sealed class IsContainedInAssertion<T>(
    AssertionContext<T> context,
    IEnumerable<T> container)
    : Assertion<T>(context)
{
    private readonly IEnumerable<T> _container = container
                                                 ?? throw new ArgumentNullException(nameof(container));

    protected override Task<AssertionResult> CheckAsync(
        EvaluationMetadata<T> metadata)
    {
        if (metadata.Exception is not null)
        {
            return Task.FromResult(
                AssertionResult.Failed(
                    $"threw {metadata.Exception.GetType().Name}"));
        }

        if (_container.Contains(metadata.Value))
        {
            return Task.FromResult(AssertionResult.Passed);
        }

        return Task.FromResult(
            AssertionResult.Failed(
                $"'{metadata.Value}' was not contained in the supplied collection"));
    }

    protected override string GetExpectation()
        => "to be contained in the supplied collection";
}

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