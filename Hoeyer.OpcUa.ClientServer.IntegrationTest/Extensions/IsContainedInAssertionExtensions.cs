using System.Runtime.CompilerServices;
using TUnit.Assertions.Core;

namespace Hoeyer.OpcUa.IntegrationTest.Extensions;

public static class IsContainedInAssertionExtensions
{
    public static IsContainedInAssertion<T> IsContainedIn<T>(
        this IAssertionSource<T> source,
        IEnumerable<T> container,
        [CallerArgumentExpression(nameof(container))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".IsContainedIn({expression})");

        return new IsContainedInAssertion<T>(
            source.Context,
            container);
    }

    public static ContainsAssertion<T> Contains<T>(
        this IAssertionSource<IEnumerable<T>> source,
        T expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".Contains({expression})");

        return new ContainsAssertion<T>(
            source.Context,
            expected);
    }
}