using System.Runtime.CompilerServices;
using Opc.Ua;
using TUnit.Assertions.Core;

namespace Hoeyer.OpcUa.Fixtures.Common.TUnit.Configuration.Assertions;

public static class VariantAssertionExtensions
{
    public static VariantEqualsAssertion<bool> VariantEquals(
        this IAssertionSource<bool> source,
        bool? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<bool>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<sbyte> VariantEquals(
        this IAssertionSource<sbyte> source,
        sbyte? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<sbyte>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<byte> VariantEquals(
        this IAssertionSource<byte> source,
        byte? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<byte>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<short> VariantEquals(
        this IAssertionSource<short> source,
        short? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<short>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<ushort> VariantEquals(
        this IAssertionSource<ushort> source,
        ushort? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<ushort>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<int> VariantEquals(
        this IAssertionSource<int> source,
        int? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<int>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<uint> VariantEquals(
        this IAssertionSource<uint> source,
        uint? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<uint>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<long> VariantEquals(
        this IAssertionSource<long> source,
        long? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<long>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<ulong> VariantEquals(
        this IAssertionSource<ulong> source,
        ulong? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<ulong>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<float> VariantEquals(
        this IAssertionSource<float> source,
        float? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<float>(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<double> VariantEquals(
        this IAssertionSource<double> source,
        double? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<double>(
            Variant.From,
            source.Context,
            expected);
    }

    public static StringVariantEqualsAssertion VariantEquals(
        this IAssertionSource<string> source,
        string? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new StringVariantEqualsAssertion(
            Variant.From,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<DateTime> VariantEquals(
        this IAssertionSource<DateTime> source,
        DateTime expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<DateTime>(
            dateTime => Variant.From(DateTimeUtc.From(dateTime)),
            source.Context,
            expected);
    }

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
    
    public sealed class StringCollectionVariantEqualsAssertion(
        Func<IEnumerable<string>, Variant> factory,
        AssertionContext<IEnumerable<string>> context,
        IEnumerable<string>? expected) : Assertion<IEnumerable<string>>(context)
    {
        protected override Task<AssertionResult> CheckAsync(
            EvaluationMetadata<IEnumerable<string>> metadata)
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

    public sealed class VariantEqualsAssertion<T>(
        Func<T, Variant> factory,
        AssertionContext<T> context,
        T? expected) : Assertion<T>(context) where T : struct
    {
        protected override Task<AssertionResult> CheckAsync(
            EvaluationMetadata<T> metadata)
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

    public sealed class StringVariantEqualsAssertion(
        Func<string, Variant> factory,
        AssertionContext<string> context,
        string? expected) : Assertion<string>(context)
    {
        protected override Task<AssertionResult> CheckAsync(
            EvaluationMetadata<string> metadata)
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

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        bool expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        sbyte expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        byte expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        short expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        ushort expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        int expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        uint expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        long expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        ulong expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        float expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        double expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        string? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expected);
    }

    public static VariantEqualsAssertion<Variant> VariantEquals(
        this IAssertionSource<Variant> source,
        DateTime expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        var expectedVariant = DateTimeUtc.From(expected);
        return new VariantEqualsAssertion<Variant>(
            variant => variant,
            source.Context,
            expectedVariant);
    }

    private static CollectionVariantEqualsAssertion<T> CreateCollectionAssertion<T>(
        IAssertionSource<IEnumerable<T>> source,
        IEnumerable<T>? expected,
        Func<IEnumerable<T>, Variant> factory,
        string? expression) where T : struct
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new CollectionVariantEqualsAssertion<T>(
            factory,
            source.Context,
            expected);
    }
    

    public static CollectionVariantEqualsAssertion<bool> VariantEquals(
        this IAssertionSource<IEnumerable<bool>> source,
        IEnumerable<bool>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<sbyte> VariantEquals(
        this IAssertionSource<IEnumerable<sbyte>> source,
        IEnumerable<sbyte>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<byte> VariantEquals(
        this IAssertionSource<IEnumerable<byte>> source,
        IEnumerable<byte>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<short> VariantEquals(
        this IAssertionSource<IEnumerable<short>> source,
        IEnumerable<short>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<ushort> VariantEquals(
        this IAssertionSource<IEnumerable<ushort>> source,
        IEnumerable<ushort>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<int> VariantEquals(
        this IAssertionSource<IEnumerable<int>> source,
        IEnumerable<int>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<uint> VariantEquals(
        this IAssertionSource<IEnumerable<uint>> source,
        IEnumerable<uint>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<long> VariantEquals(
        this IAssertionSource<IEnumerable<long>> source,
        IEnumerable<long>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<ulong> VariantEquals(
        this IAssertionSource<IEnumerable<ulong>> source,
        IEnumerable<ulong>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<float> VariantEquals(
        this IAssertionSource<IEnumerable<float>> source,
        IEnumerable<float>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static CollectionVariantEqualsAssertion<double> VariantEquals(
        this IAssertionSource<IEnumerable<double>> source,
        IEnumerable<double>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(values.ToArrayOf()),
            expression);
    }

    public static StringCollectionVariantEqualsAssertion VariantEquals(
        this IAssertionSource<IEnumerable<string>> source,
        IEnumerable<string>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        source.Context.ExpressionBuilder.Append(
            $".VariantEquals({expression})");

        return new StringCollectionVariantEqualsAssertion(
            s => Variant.From(s.ToArrayOf()),
            source.Context,
            expected);
    }

    public static CollectionVariantEqualsAssertion<DateTime> VariantEquals(
        this IAssertionSource<IEnumerable<DateTime>> source,
        IEnumerable<DateTime>? expected,
        [CallerArgumentExpression(nameof(expected))]
        string? expression = null)
    {
        return CreateCollectionAssertion(
            source,
            expected,
            values => Variant.From(
                values.Select(DateTimeUtc.From).ToArrayOf()),
            expression);
    }
}