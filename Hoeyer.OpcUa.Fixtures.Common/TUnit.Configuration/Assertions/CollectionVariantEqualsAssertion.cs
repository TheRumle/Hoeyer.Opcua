using Opc.Ua;
using TUnit.Assertions.Core;

namespace Hoeyer.OpcUa.Fixtures.Common.TUnit.Configuration.Assertions;

public sealed class CollectionVariantEqualsAssertion<T>(
    Func<IEnumerable<T>, Variant> factory,
    AssertionContext<IEnumerable<T>> context,
    IEnumerable<T>? expected) : Assertion<IEnumerable<T>>(context) where T : struct
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

        var actualVariant = factory.Invoke(metadata.Value);
        var other = expected == null ? Variant.Null : factory.Invoke(metadata.Value);
        if (actualVariant.ValueEquals(other))
        {
            return Task.FromResult(AssertionResult.Passed);
        }

        return Task.FromResult(
            AssertionResult.Failed(
                $"Expected variant value to equal '{expected}', " +
                $"but was '{metadata.Value}'"));
    }

    protected override string GetExpectation() => $"to have a variant value equal to '{expected}'";
}