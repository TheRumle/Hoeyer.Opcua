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